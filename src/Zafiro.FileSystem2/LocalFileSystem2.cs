using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.IO;
using Zafiro.Mixins;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Zafiro.FileSystem2;

public class LocalFileSystem2 : IFileSystem2
{
    private readonly System.IO.Abstractions.IFileSystem fileSystem;

    public LocalFileSystem2(System.IO.Abstractions.IFileSystem fileSystem)
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
    public async Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path) => Result.Try(() => fileSystem.Directory.GetDirectories(path).Select(s => (ZafiroPath)s));
    public async Task<Result<bool>> ExistDirectory(ZafiroPath path) => Result.Try(() => fileSystem.Directory.Exists(path));
    public async Task<Result<bool>> ExistFile(ZafiroPath path) => Result.Try(() => fileSystem.File.Exists(path));
    public async Task<Result> DeleteFile(ZafiroPath path) => Result.Try(() => fileSystem.Directory.Delete(path));
    public async Task<Result> DeleteDirectory(ZafiroPath path) => Result.Try(() => fileSystem.Directory.Delete(path, true));
}