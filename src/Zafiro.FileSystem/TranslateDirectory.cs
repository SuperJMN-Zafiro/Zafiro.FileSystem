using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Comparer;

namespace Zafiro.FileSystem;

public class TranslateDirectory : IZafiroDirectory
{
    public IZafiroDirectory Directory { get; }
    public IZafiroDirectory Root { get; }
    public Func<ZafiroPath, ZafiroPath> TranslatePath { get; }
    public Func<IZafiroDirectory, IZafiroDirectory> CreateDirectory { get; }
    public Func<IZafiroFile, IZafiroFile> CreateFile { get; }

    public TranslateDirectory(IZafiroDirectory directory, IZafiroDirectory root, Func<ZafiroPath, ZafiroPath> translatePath, Func<IZafiroDirectory, IZafiroDirectory> createDirectory, Func<IZafiroFile, IZafiroFile> createFile)
    {
        Directory = directory;
        Root = root;
        TranslatePath = translatePath;
        CreateDirectory = createDirectory;
        CreateFile = createFile;
    }

    public ZafiroPath Path => TranslatePath(Directory.Path);
    public Task<Result<bool>> Exists => Directory.Exists;
    public IFileSystemRoot FileSystem => Root.FileSystem;
    public Task<Result<DirectoryProperties>> Properties => Root.Properties;
    public IObservable<FileSystemChange> Changed => Root.Changed.Select(change => change with { Path = TranslatePath(Path) });

    public Task<Result> Create() => Root.Create();

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles() => Directory.GetFiles().MapMany(CreateFile);

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories() => Directory
        .GetDirectories()
        .MapMany(CreateDirectory);

    public Task<Result> Delete() => Directory.Delete();

    public override string ToString() => $"{Path} ({Directory.Path})";
}