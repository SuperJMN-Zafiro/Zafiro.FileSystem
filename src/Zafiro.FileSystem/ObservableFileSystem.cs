using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using MoreLinq;

namespace Zafiro.FileSystem;

public class ObservableFileSystem : IObservableFileSystem
{
    private readonly Subject<FileSystemChange> changed = new();
    private readonly IZafiroFileSystem fs;

    public ObservableFileSystem(IZafiroFileSystem fs)
    {
        this.fs = fs;
    }

    public IObservable<FileSystemChange> Changed => changed.AsObservable();
    public Task<Result<bool>> ExistsDirectory(ZafiroPath path) => fs.ExistDirectory(path);
    public Task<Result<bool>> ExistFile(ZafiroPath path) => fs.ExistFile(path);
    public Task<Result> DeleteFile(ZafiroPath path) => fs.DeleteFile(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.FileDeleted)));
    public Task<Result> DeleteDirectory(ZafiroPath path) => fs.DeleteDirectory(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.DirectoryDeleted)));

    public async Task<Result> CreateFile(ZafiroPath path)
    {
        var parents = await path
            .Parents()
            .Select(zafiroPath => fs.ExistDirectory(zafiroPath).Map(b => (File: zafiroPath, Exist: b)))
            .Combine()
            .ConfigureAwait(false);

        return await fs.CreateFile(path)
            .Tap(() => changed.OnNext(new FileSystemChange(path, Change.FileCreated)))
            .Tap(() => parents.Tap(par => par.Where(r => !r.Exist).ForEach(tuple => changed.OnNext(new FileSystemChange(tuple.File, Change.DirectoryCreated)))))
            .ConfigureAwait(false);
    }

    public IObservable<byte> GetFileContents(ZafiroPath path) => fs.GetFileContents(path);

    public async Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes)
    {
        var changes = await Notifications.BeforeFileCreate(fs, changed, path).ConfigureAwait(false);

        return await fs
            .SetFileContents(path, bytes)
            .Tap(() => changes.ForEach(r => changed.OnNext(r)))
            .ConfigureAwait(false);
    }

    public Task<Result> CreateDirectory(ZafiroPath path) => fs.CreateDirectory(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.DirectoryCreated)));
    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path) => fs.GetFileProperties(path);
    public Task<Result<IDictionary<ChecksumKind, byte[]>>> GetChecksums(ZafiroPath path) => fs.GetChecksums(path);

    public Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path) => fs.GetDirectoryProperties(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = default) => fs.GetFilePaths(path, ct);
    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default) => fs.GetDirectoryPaths(path, ct);
    public Task<Result<bool>> ExistDirectory(ZafiroPath path) => fs.ExistDirectory(path);
}

public enum Change
{
    FileCreated,
    FileDeleted,
    DirectoryCreated,
    FileContentsChanged,
    DirectoryDeleted
}

public record FileSystemChange(ZafiroPath Path, Change Change);