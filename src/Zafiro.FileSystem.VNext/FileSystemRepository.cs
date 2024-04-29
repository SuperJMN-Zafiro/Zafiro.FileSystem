﻿using System.IO;
using System.IO.Abstractions;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Lightweight;
using Zafiro.Mixins;
using IDirectory = Zafiro.FileSystem.Lightweight.IDirectory;
using IFile = Zafiro.FileSystem.Lightweight.IFile;

namespace Zafiro.FileSystem.VNext;

public class FileSystemRepository : IFileRepository
{
    private readonly IFileSystem fileSystem;

    public FileSystemRepository(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public Task<Result<Maybe<IFile>>> GetFile(ZafiroPath path)
    {
        return GetFiles(path.Parent()).Map(files => files.TryFirst(file => file.Name == path.Name()));
    }

    public Task<Result<Maybe<IDirectory>>> GetDirectory(ZafiroPath path)
    {
        return GetDirectories(path.Parent()).Map(files => files.TryFirst(file => file.Name == path.Name()));
    }

    public Task<Result<IEnumerable<IFile>>> GetFiles(ZafiroPath path)
    {
        return Task.FromResult(Result.Try(() =>
        {
            var dir = fileSystem.DirectoryInfo.New(path);
            return dir.GetFiles()
                .Select(fileInfo => (IFile) new RootedFile(dir.FullName.Replace("\\", "/"), new SystemIOFile(fileInfo)));
        }));
    }

    public Task<Result<IEnumerable<IDirectory>>> GetDirectories(ZafiroPath path)
    {
        return Task.FromResult(Result.Try(() =>
        {
            var rootDir = path == ZafiroPath.Empty ? fileSystem.DriveInfo.GetDrives().Select(r => r.RootDirectory) : fileSystem.DirectoryInfo.New(path).GetDirectories();
        
            return rootDir
                .Select(s => (IDirectory) new SystemIODirectory(s)); 
        }));
    }

    public async Task<Result<IFile>> AddOrUpdate(ZafiroPath path, IFile data)
    {
        var result = path.Combine(data.Name);
        return await WriteStream(result.ToString().Replace("/", "\\"), data)
            .Bind(() => GetFile(result)
                .Bind(r => r.ToResult("Could not retrieve the file")));
    }
    
    public async Task<Result> WriteStream(string ruta, IByteProvider byteProvider)
    {
        // Extraer el directorio padre de la ruta proporcionada
        var directorio = Path.GetDirectoryName(ruta);

        // Verificar si el directorio existe y crearlo si no existe
        if (!fileSystem.Directory.Exists(directorio))
        {
            fileSystem.Directory.CreateDirectory(directorio);
        }

        // Crear el archivo en la ruta especificada, FileMode.Create crea un nuevo archivo
        // o sobrescribe uno existente
        await using (var fs = fileSystem.File.Create(ruta))
        {
            return (await byteProvider.DumpTo(fs).ToList()).Combine();
        }
    }
}

public static class ResultMixin
{
    public static async Task<Result> Using(this Task<Result<Stream>> streamResult, Func<Stream, Task> useStream)
    {
        return await streamResult.Tap(async stream =>
        {
            await using (stream)
            {
                await useStream(stream);
            }
        });
    }
}