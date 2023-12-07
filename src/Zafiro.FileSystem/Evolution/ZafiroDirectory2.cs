using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Evolution;

public class ZafiroDirectory2 : IZafiroDirectory2
{
    public ZafiroPath Path { get; }
    public IFileSystemRoot FileSystemRoot { get; }

    public ZafiroDirectory2(ZafiroPath path, IFileSystemRoot fileSystemRoot)
    {
        this.Path = path;
        this.FileSystemRoot = fileSystemRoot;
    }

    public Task<Result> Create() => FileSystemRoot.CreateDirectory(Path);
    public Task<Result<IEnumerable<IZafiroFile2>>> GetFiles() => FileSystemRoot.GetFiles(Path);
    public Task<Result<IEnumerable<IZafiroDirectory2>>> GetDirectories() => FileSystemRoot.GetDirectories(Path);
    public Task<Result<bool>> Exists => FileSystemRoot.ExistDirectory(Path);
}