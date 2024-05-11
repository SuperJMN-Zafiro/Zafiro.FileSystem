using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Lightweight;

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

    public Task<Result<Maybe<IHeavyDirectory>>> GetDirectory(ZafiroPath path)
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

    public Task<Result<IEnumerable<IHeavyDirectory>>> GetDirectories(ZafiroPath path)
    {
        return Task.FromResult(Result.Try(() =>
        {
            var rootDir = path == ZafiroPath.Empty ? fileSystem.DriveInfo.GetDrives().Select(r => r.RootDirectory) : fileSystem.DirectoryInfo.New(path).GetDirectories();
        
            return rootDir
                .Select(s => (IHeavyDirectory) new SystemIOHeavyDirectory(s)); 
        }));
    }

    public async Task<Result<IFile>> AddOrUpdate(ZafiroPath path, IFile data)
    {
        var result = path.Combine(data.Name);
        return await WriteStream(result.ToString().Replace("/", "\\"), data)
            .Bind(() => GetFile(result)
                .Bind(r => r.ToResult("Could not retrieve the file")));
    }
    
    public async Task<Result> WriteStream(string ruta, IData byteProvider)
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
            return await byteProvider.DumpTo(fs);
        }
    }
}