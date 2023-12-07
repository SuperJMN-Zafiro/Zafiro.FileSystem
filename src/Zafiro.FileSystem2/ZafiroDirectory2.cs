using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.FileSystem2;

public class ZafiroDirectory2 : IZafiroDirectory2
{
    public ZafiroPath Path { get; }
    private readonly IFileSystemRoot fileSystemRoot;

    public ZafiroDirectory2(ZafiroPath path, IFileSystemRoot fileSystemRoot)
    {
        this.Path = path;
        this.fileSystemRoot = fileSystemRoot;
    }

    public Task<Result> Create() => fileSystemRoot.CreateDirectory(Path);
    public Task<Result<bool>> Exists => fileSystemRoot.ExistDirectory(Path);
}