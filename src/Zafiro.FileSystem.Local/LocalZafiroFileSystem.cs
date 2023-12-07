﻿using System.Reactive.Linq;
using Zafiro.IO;
using Zafiro.Mixins;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Zafiro.FileSystem.Local;

public class LocalZafiroFileSystem : IZafiroFileSystem
{
    private readonly System.IO.Abstractions.IFileSystem fileSystem;

    public LocalZafiroFileSystem(System.IO.Abstractions.IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public async Task<Result> CreateFile(ZafiroPath path)
    {
        return Result.Try(() => fileSystem.File.Create(path));
    }

    public IObservable<byte> Contents(ZafiroPath path)
    {
        return Observable.Using(() => fileSystem.File.OpenRead(path), f => f.ToObservable());
    }

    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes)
    {
        return Result
            .Try(() => fileSystem.File.Open(path, FileMode.Create))
            .Bind(stream => Result.Try(async () =>
            {
                using (stream)
                {
                    await bytes.ToStream().CopyToAsync(stream);
                }
            }));
    }

    public async Task<Result> CreateDirectory(ZafiroPath path)
    {
        return Result.Try(() => fileSystem.Directory.CreateDirectory(path));
    }

    public async Task<Result<FileProperties>> GetFileProperties(ZafiroPath path)
    {
        return Result.Try(() =>
        {
            var info = fileSystem.FileInfo.New(path);
            return new FileProperties(info.Attributes.HasFlag(FileAttributes.Hidden), info.CreationTime, info.Length);
        });
    }

    public async Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path) => Result.Try(() => fileSystem.Directory.GetFiles(path).Select(s => (ZafiroPath)s));
    public async Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path) => Result.Try(() => GetDirectories(path).Select(s => (ZafiroPath)s));

    private IEnumerable<ZafiroPath> GetDirectories(ZafiroPath path)
    {
        if (path == ZafiroPath.Empty)
        {
            return fileSystem.DriveInfo.GetDrives().Select(i => i.RootDirectory.FullName[..^1]).Select(x => x.ToZafiroPath());
        }

        return fileSystem.Directory.GetDirectories(path).Select(x => x.ToZafiroPath());
    }

    public async Task<Result<bool>> ExistDirectory(ZafiroPath path) => Result.Try(() => fileSystem.Directory.Exists(path));
    public async Task<Result<bool>> ExistFile(ZafiroPath path) => Result.Try(() => fileSystem.File.Exists(path));
    public async Task<Result> DeleteFile(ZafiroPath path) => Result.Try(() => fileSystem.Directory.Delete(path));
    public async Task<Result> DeleteDirectory(ZafiroPath path) => Result.Try(() => fileSystem.Directory.Delete(path, true));
}