using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Renci.SshNet;
using Zafiro.IO;
using Zafiro.Mixins;

namespace Zafiro.FileSystem.Sftp;

public class SftpFileSystem : IZafiroFileSystem
{
    private readonly SftpClient sftpClient;

    public SftpFileSystem(SftpClient sftpClient)
    {
        this.sftpClient = sftpClient;
    }

    public async Task<Result> CreateFile(ZafiroPath path) => Result.Try(() => sftpClient.Create(FromZafiroToFileSystem(path)));

    private static string FromZafiroToFileSystem(ZafiroPath path)
    {
        return "/" + path.Path;
    }

    public static ZafiroPath FromFileSystemToZafiroPath(string path)
    {
        return path.StartsWith("/") ? path[1..] : path;
    }

    public IObservable<byte> GetFileContents(ZafiroPath path)
    {
        return Observable.Using(() => sftpClient.OpenRead(FromZafiroToFileSystem(path)), stream => stream.ToObservable());
    }

    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes)
    {
        return EnsureDirectoryExists(path.Parent())
            .Bind(async () => await Observable.Using(() => bytes.ToStream(), stream => Observable.FromAsync(ct => Result.Try(() => sftpClient.UploadFileAsync(FromFileSystemToZafiroPath(path), stream)))));
    }

    public async Task<Result> CreateDirectory(ZafiroPath path) => Result.Try(() => sftpClient.Create(FromZafiroToFileSystem(path)));

    public async Task<Result<FileProperties>> GetFileProperties(ZafiroPath path)
    {
        return Result
            .Try(() => sftpClient.GetAttributes(FromZafiroToFileSystem(path)))
            .Map(f => new FileProperties(path.Name().StartsWith("."), DateTimeOffset.MinValue, f.Size, new Dictionary<ChecksumKind, byte[]>()));
    }

    public async Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path)
    {
        return Result
            .Try(() => sftpClient.GetAttributes(FromZafiroToFileSystem(path)))
            .Map(_ => new DirectoryProperties(path.Name().StartsWith("."), DateTimeOffset.MinValue));
    }

    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = default)
    {
        return Result.Try(() => sftpClient.ListDirectoryAsync(FromZafiroToFileSystem(path)))
            .Map(files => files
                .Where(file => !file.IsDirectory)
                .Select(file => FromFileSystemToZafiroPath(file.FullName)));
    }

    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default)
    {
        return Result.Try(() => sftpClient.ListDirectoryAsync(FromZafiroToFileSystem(path)))
            .Map(files => files
                .Where(file => file.IsDirectory)
                .Select(file => FromFileSystemToZafiroPath(file.FullName)));
    }

    public async Task<Result<bool>> ExistDirectory(ZafiroPath path) => Result.Try(() => sftpClient.Exists(FromFileSystemToZafiroPath(path)));

    public async Task<Result<bool>> ExistFile(ZafiroPath path) => Result.Try(() => sftpClient.Exists(FromFileSystemToZafiroPath(path)));

    public async  Task<Result> DeleteFile(ZafiroPath path) => Result.Try(() => sftpClient.DeleteFile(FromFileSystemToZafiroPath(path)));

    public async Task<Result> DeleteDirectory(ZafiroPath path) => Result.Try(() => sftpClient.DeleteDirectory(FromFileSystemToZafiroPath(path)));

    private Result EnsureDirectoryExists(ZafiroPath path)
    {
        return Result.Try(() =>
        {
            if (!sftpClient.Exists(path))
            {
                EnsureDirectoryExists(path.Parent());
                sftpClient.CreateDirectory(path);
            }
        });
    }
}