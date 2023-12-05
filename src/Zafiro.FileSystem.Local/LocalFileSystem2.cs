using System.Reactive.Linq;
using Zafiro.IO;
using Zafiro.Mixins;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Zafiro.FileSystem.Local;

public class LocalFileSystem2
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

    public async Task<Result<IObservable<byte>>> GetFileContents(ZafiroPath path)
    {
        return Result.Try(() => Observable.Using(() => fileSystem.File.OpenRead(path), f => f.ToObservable()));
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

    public async Task<Result> CreateFolder(ZafiroPath path)
    {
        return Result.Try(() => fileSystem.Directory.CreateDirectory(path));
    }

    public async Task<Result<FileProperties>> GetFileProperties(ZafiroPath path)
    {
        return Result.Try(() =>
        {
            var info = fileSystem.FileInfo.New(path);
            return new FileProperties()
            {
                IsHidden = info.Attributes.HasFlag(FileAttributes.Hidden),
                CreationTime = info.CreationTime,
                Length = info.Length,
            };
        });
    }
}