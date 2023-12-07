using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Evolution;

public class ObservableFileSystem : IObservableFileSystem
{
    private readonly Subject<FileSystemChange> changed = new();
    private readonly IFileSystem2 fs;

    public ObservableFileSystem(IFileSystem2 fs)
    {
        this.fs = fs;
    }

    public IObservable<FileSystemChange> Changed => changed.AsObservable();
    public Task<Result<bool>> ExistsDirectory(ZafiroPath path) => fs.ExistDirectory(path);
    public Task<Result<bool>> ExistFile(ZafiroPath path) => fs.ExistFile(path);
    public Task<Result> DeleteFile(ZafiroPath path) => fs.DeleteFile(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.FileDeleted)));
    public Task<Result> DeleteDirectory(ZafiroPath path) => fs.DeleteDirectory(path);

    public Task<Result> CreateFile(ZafiroPath path) => fs.CreateFile(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.FileCreated)));
    public IObservable<byte> Contents(ZafiroPath path) => fs.Contents(path);
    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes) => fs.SetFileContents(path, bytes).Tap(() => changed.OnNext(new FileSystemChange(path, Change.FileContentsChanged)));
    public Task<Result> CreateDirectory(ZafiroPath path) => fs.CreateDirectory(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.DirectoryCreated)));
    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path) => fs.GetFileProperties(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path) => fs.GetFilePaths(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path) => fs.GetDirectoryPaths(path);
    public Task<Result<bool>> ExistDirectory(ZafiroPath path) => fs.ExistDirectory(path);
}

public enum Change
{
    FileCreated,
    FileDeleted,
    DirectoryCreated,
    FileContentsChanged
}

public record FileSystemChange(ZafiroPath Path, Change Change);