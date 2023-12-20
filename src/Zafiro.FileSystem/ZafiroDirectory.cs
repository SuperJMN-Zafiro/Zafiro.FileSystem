using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class ZafiroDirectory : IZafiroDirectory
{
    public ZafiroPath Path { get; }
    public IFileSystemRoot FileSystem { get; }
    public Task<Result<DirectoryProperties>> Properties => FileSystem.GetDirectoryProperties(Path);

    public ZafiroDirectory(ZafiroPath path, IFileSystemRoot fileSystemRoot)
    {
        Path = path;
        FileSystem = fileSystemRoot;
        Changed = fileSystemRoot.Changed.Where(change => change.Path.Parent() == Path);
    }

    public IObservable<FileSystemChange> Changed { get; }
    public Task<Result> Create() => FileSystem.CreateDirectory(Path);
    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles() => FileSystem.GetFiles(Path);
    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories() => FileSystem.GetDirectories(Path);
    public Task<Result> Delete() => FileSystem.DeleteDirectory(Path);
    public Task<Result<bool>> Exists => FileSystem.ExistDirectory(Path);

    public override string ToString() => $"{Path}";
}