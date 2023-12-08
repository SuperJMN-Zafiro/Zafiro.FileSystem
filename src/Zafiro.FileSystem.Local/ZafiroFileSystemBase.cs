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

    public async Task<Result> CreateFile(ZafiroPath path)
    {
        return Result.Try(() => FileSystem.File.Create(PathToFileSystem(path)));
    }

    public virtual IObservable<byte> GetFileContents(ZafiroPath path)
    {
        return Observable.Using(() => FileSystem.File.OpenRead(PathToFileSystem(path)), f => f.ToObservable());
    }

    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes)
    {
        return Result
            .Try(() => FileSystem.File.Open(PathToFileSystem(path), FileMode.Create))
            .Bind(stream => Result.Try(async () =>
            {
                using (stream)
                {
                    await bytes.ToStream().CopyToAsync(stream);
                }
            }));
    }

    public async Task<Result> CreateDirectory(ZafiroPath path)
    {
        return Result.Try(() => FileSystem.Directory.CreateDirectory(PathToFileSystem(path)));
    }

    public async Task<Result<FileProperties>> GetFileProperties(ZafiroPath path)
    {
        return Result.Try(() =>
        {
            var info = FileSystem.FileInfo.New(PathToFileSystem(path));
            return new FileProperties(info.Attributes.HasFlag(FileAttributes.Hidden), info.CreationTime, info.Length);
        });
    }

    public async Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path) => Result.Try(() => FileSystem.Directory.GetFiles(PathToFileSystem(path)).Select(s => (ZafiroPath) s));
    public async Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path) => Result.Try(() => GetDirectories(PathToFileSystem(path)).Select(s => FileSystemToZafiroPath(s)));
    public async Task<Result<bool>> ExistDirectory(ZafiroPath path) => Result.Try(() => FileSystem.Directory.Exists(path));
    public async Task<Result<bool>> ExistFile(ZafiroPath path) => Result.Try(() => FileSystem.File.Exists(path));
    public async Task<Result> DeleteFile(ZafiroPath path) => Result.Try(() => FileSystem.Directory.Delete(path));
    public async Task<Result> DeleteDirectory(ZafiroPath path) => Result.Try(() => FileSystem.Directory.Delete(path, true));

    public abstract string PathToFileSystem(ZafiroPath path);
    public abstract ZafiroPath FileSystemToZafiroPath(string fileSystemPath);
    protected abstract IEnumerable<ZafiroPath> GetDirectories(ZafiroPath path);
}