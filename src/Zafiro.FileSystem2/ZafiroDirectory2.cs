using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.FileSystem2;

public class ZafiroDirectory2 : IZafiroDirectory2
{
    private readonly ZafiroPath path;
    private readonly IFileSystemRoot fileSystemRoot;

    public ZafiroDirectory2(ZafiroPath path, IFileSystemRoot fileSystemRoot)
    {
        this.path = path;
        this.fileSystemRoot = fileSystemRoot;
    }

    public Task<Result> Create() => fileSystemRoot.CreateFolder(path);

    public Task<Result<bool>> Exists => fileSystemRoot.ExistDirectory(path);
}