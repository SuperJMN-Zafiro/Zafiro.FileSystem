using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Comparer;

namespace Zafiro.FileSystem;

public class RootedDirectory : IZafiroDirectory
{
    private readonly IZafiroDirectory dir;
    private readonly IZafiroDirectory root;

    public RootedDirectory(IZafiroDirectory dir, IZafiroDirectory root)
    {
        this.root = root;
        this.dir = dir;
    }

    public ZafiroPath Path => dir.Path.MakeRelativeTo(root.Path);
    public Task<Result<bool>> Exists => dir.Exists;
    public IFileSystemRoot FileSystem => root.FileSystem;
    public Task<Result<DirectoryProperties>> Properties => root.Properties;
    public IObservable<FileSystemChange> Changed => root.Changed.Select(change => change with { Path = Path.MakeRelativeTo(root.Path) });
    public Task<Result> Create() => root.Create();

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles() => dir.GetFiles().Map(file => (IZafiroFile) new RootedFile(file, root));

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories() => dir
        .GetDirectories()
        .Map(directory => (IZafiroDirectory) new RootedDirectory(directory, root));

    public Task<Result> Delete() => dir.Delete();
}