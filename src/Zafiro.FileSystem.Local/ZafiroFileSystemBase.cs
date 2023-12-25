using System.IO.Abstractions;
using System.Reactive.Linq;
using Zafiro.IO;
using Zafiro.Mixins;

namespace Zafiro.FileSystem.Local;

public abstract class ZafiroFileSystemBase : IZafiroFileSystem
{
    protected readonly IFileSystem FileSystem;

    public ZafiroFileSystemBase(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public virtual async Task<Result> CreateFile(ZafiroPath path)
    {
        return Result.Try(() => { FileSystem.File.Create(PathToFileSystem(path)); });
    }

    public virtual IObservable<byte> GetFileContents(ZafiroPath path)
    {
        return Observable.Using(() => FileSystem.File.OpenRead(PathToFileSystem(path)), f => f.ToObservable());
    }

    public virtual Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes)
    {
        return Result
            .Try(() =>
            {
                EnsureExist(PathToFileSystem(path.Parent()));
                return FileSystem.File.Open(PathToFileSystem(path), FileMode.Create);
            })
            .Bind(stream => Result.Try(async () =>
            {
                using (stream)
                {
                    await bytes.ToStream().CopyToAsync(stream);
                }
            }));
    }

    public virtual async Task<Result> CreateDirectory(ZafiroPath path)
    {
        return Result.Try(() => FileSystem.Directory.CreateDirectory(PathToFileSystem(path)));
    }

    public virtual async Task<Result<FileProperties>> GetFileProperties(ZafiroPath path)
    {
        return Result.Try(() =>
        {
            var info = FileSystem.FileInfo.New(PathToFileSystem(path));
            return new FileProperties(info.Attributes.HasFlag(FileAttributes.Hidden), info.CreationTime, info.Length);
        });
    }

    public async Task<Result<IDictionary<ChecksumKind, byte[]>>> GetChecksums(ZafiroPath path)
    {
        return new Dictionary<ChecksumKind, byte[]>();
    }

    public virtual async Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path)
    {
        return Result.Try(() =>
        {
            var info = FileSystem.FileInfo.New(PathToFileSystem(path));
            var isHidden = info.Attributes.HasFlag(FileAttributes.Hidden);
            return new DirectoryProperties(isHidden, info.CreationTime);
        });
    }

    public virtual async Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = default)
    {
        if (path == ZafiroPath.Empty)
        {
            return Result.Success(Enumerable.Empty<ZafiroPath>());
        }

        return Result.Try(() => FileSystem.Directory.GetFiles(PathToFileSystem(path)).Select(FileSystemToZafiroPath));
    }

    public virtual async Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default)
    {
        return Result.Try(() => FileSystem.Directory.GetDirectories(PathToFileSystem(path)).Select(FileSystemToZafiroPath));
    }

    public async Task<Result<bool>> ExistDirectory(ZafiroPath path) => Result.Try(() => FileSystem.Directory.Exists(path));
    public async Task<Result<bool>> ExistFile(ZafiroPath path) => Result.Try(() => FileSystem.File.Exists(path));
    public async Task<Result> DeleteFile(ZafiroPath path) => Result.Try(() => FileSystem.File.Delete(path));
    public async Task<Result> DeleteDirectory(ZafiroPath path) => Result.Try(() => FileSystem.Directory.Delete(path, true));
    public abstract string PathToFileSystem(ZafiroPath path);
    public abstract ZafiroPath FileSystemToZafiroPath(string fileSystemPath);

    private void EnsureExist(string path)
    {
        if (!FileSystem.Directory.Exists(path))
        {
            FileSystem.Directory.CreateDirectory(path);
        }
    }
}
