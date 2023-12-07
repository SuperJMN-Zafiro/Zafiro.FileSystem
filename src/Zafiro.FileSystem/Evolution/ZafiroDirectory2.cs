using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Evolution;

public class ZafiroDirectory2 : IZafiroDirectory2
{
    public ZafiroPath Path { get; }
    public IFileSystemRoot FileSystem { get; }

    public ZafiroDirectory2(ZafiroPath path, IFileSystemRoot fileSystemRoot)
    {
        this.Path = path;
        this.FileSystem = fileSystemRoot;
    }

    public Task<Result> Create() => FileSystem.CreateDirectory(Path);
    public Task<Result<IEnumerable<IZafiroFile2>>> GetFiles() => FileSystem.GetFiles(Path);
    public Task<Result<IEnumerable<IZafiroDirectory2>>> GetDirectories() => FileSystem.GetDirectories(Path);
    public Task<Result> Delete() => FileSystem.DeleteDirectory(Path);
    public Task<Result<bool>> Exists => FileSystem.ExistDirectory(Path);
}