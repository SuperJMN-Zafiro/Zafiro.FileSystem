using System.IO.Abstractions;
using System.Reactive.Linq;
using System.Security.Cryptography;
using Zafiro.Reactive;

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
        return Observable.Using(() => FileSystem.File.OpenRead(PathToFileSystem(path)), f => f.ToObservable(1024 * 1024));
    }

    public Task<Result> SetFileData(ZafiroPath path, Stream stream, CancellationToken cancellationToken = default)
    {
        return Result
            .Try(() =>
            {
                EnsureExist(PathToFileSystem(path.Parent()));
                return FileSystem.File.Open(PathToFileSystem(path), FileMode.Create);
            })
            .Bind(destinationStream => Result.Try(async () =>
            {
                await using (destinationStream.ConfigureAwait(false))
                {
                    await stream.CopyToAsync(destinationStream, cancellationToken);
                }
            }));
    }

    public virtual Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes, CancellationToken cancellationToken = default)
    {
        return Result
            .Try(() =>
            {
                EnsureExist(PathToFileSystem(path.Parent()));
                return FileSystem.File.Open(PathToFileSystem(path), FileMode.Create);
            })
            .Bind(async destinationStream =>
            {
                await using (destinationStream.ConfigureAwait(false))
                {
                    return (await bytes.DumpTo(destinationStream, bufferSize: 10 * 1014 * 1014).ToList()).Combine();
                }
            });
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

    public async Task<Result<IDictionary<HashMethod, byte[]>>> GetHashes(ZafiroPath path)
    {
        var pathToFileSystem = PathToFileSystem(path);
        byte[] md5;
        await using (var fileSystemStream = FileSystem.File.OpenRead(pathToFileSystem))
        {
            md5 = await MD5.HashDataAsync(fileSystemStream).ConfigureAwait(false);
        }

        byte[] sha256;
        await using (var fileSystemStream = FileSystem.File.OpenRead(pathToFileSystem))
        {
            sha256 = await SHA256.HashDataAsync(fileSystemStream).ConfigureAwait(false);
        }

        return new Dictionary<HashMethod, byte[]>
        {
            [HashMethod.Md5] = md5,
            [HashMethod.Sha256] = sha256
        };
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
    public async Task<Result<Stream>> GetFileData(ZafiroPath path) => Result.Try(() => (Stream) FileSystem.File.OpenRead(PathToFileSystem(path)));
    public abstract string PathToFileSystem(ZafiroPath path);
    public abstract ZafiroPath FileSystemToZafiroPath(string fileSystemPath);

    private void EnsureExist(string path)
    {
        if (path == "")
        {
            return;
        }

        if (!FileSystem.Directory.Exists(path))
        {
            FileSystem.Directory.CreateDirectory(path);
        }
    }
}