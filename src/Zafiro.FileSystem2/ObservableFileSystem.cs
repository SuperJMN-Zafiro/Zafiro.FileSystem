using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.FileSystem2;

public class ObservableFileSystem : IObservableFileSystem
{
    private readonly Subject<ZafiroPath> fileContentsChanged = new();
    private readonly Subject<ZafiroPath> fileCreated = new();
    private readonly Subject<ZafiroPath> folderCreated = new();
    private readonly IFileSystem2 fs;

    public ObservableFileSystem(IFileSystem2 fs)
    {
        this.fs = fs;
    }

    public IObservable<ZafiroPath> FileContentsChanged => fileContentsChanged.AsObservable();

    public IObservable<ZafiroPath> FileCreated => fileCreated.AsObservable();

    public IObservable<ZafiroPath> FolderCreated => folderCreated.AsObservable();

    public Task<Result> CreateFile(ZafiroPath path) => fs.CreateFile(path).Tap(() => fileCreated.OnNext(path));

    public IObservable<byte> Contents(ZafiroPath path) => fs.Contents(path);

    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes) => fs.SetFileContents(path, bytes).Tap(() => fileContentsChanged.OnNext(path));

    public Task<Result> CreateFolder(ZafiroPath path) => fs.CreateFolder(path).Tap(() => folderCreated.OnNext(path));

    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path) => fs.GetFileProperties(path).Tap(() => fileCreated.OnNext(path));
}