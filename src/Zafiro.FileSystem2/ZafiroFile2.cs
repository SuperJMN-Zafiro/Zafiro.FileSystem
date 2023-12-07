using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.FileSystem2;

public class ZafiroFile2 : IZafiroFile2
{
    private readonly IFileSystemRoot fileSystemRoot;

    public ZafiroFile2(ZafiroPath path, IFileSystemRoot fileSystemRoot)
    {
        Path = path;
        this.fileSystemRoot = fileSystemRoot;
    }

    public IObservable<byte> Contents => fileSystemRoot.Contents(Path);
    public Task<Result<bool>> Exists => fileSystemRoot.ExistFile(Path);
    public ZafiroPath Path { get; }
    public Task<Result> Delete() => fileSystemRoot.DeleteFile(Path);
}