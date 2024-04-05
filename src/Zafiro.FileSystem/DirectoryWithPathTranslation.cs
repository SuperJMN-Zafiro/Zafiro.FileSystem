using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Comparer;

namespace Zafiro.FileSystem;

public abstract class DirectoryWithPathTranslation : IZafiroDirectory
{
    private readonly IZafiroDirectory dir;
    private readonly IZafiroDirectory root;

    public DirectoryWithPathTranslation(IZafiroDirectory dir, IZafiroDirectory root)
    {
        this.root = root;
        this.dir = dir;
    }

    public ZafiroPath Path => TranslatePath(Dir.Path);
    public Task<Result<bool>> Exists => Dir.Exists;
    public IFileSystemRoot FileSystem => Root.FileSystem;
    public Task<Result<DirectoryProperties>> Properties => Root.Properties;
    public IObservable<FileSystemChange> Changed => Root.Changed.Select(change => change with { Path = TranslatePath(Path) });

    protected IZafiroDirectory Dir => dir;

    protected IZafiroDirectory Root => root;

    public Task<Result> Create() => Root.Create();

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles() => Dir.GetFiles().MapMany(CreateFile);

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories() => Dir
        .GetDirectories()
        .MapMany(CreateDirectory);

    public Task<Result> Delete() => Dir.Delete();

    protected abstract ZafiroPath TranslatePath(ZafiroPath path);

    protected abstract IZafiroFile CreateFile(IZafiroFile file);

    protected abstract IZafiroDirectory CreateDirectory(IZafiroDirectory directory);
}