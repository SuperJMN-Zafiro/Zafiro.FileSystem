using CSharpFunctionalExtensions;
using Serilog;
using System.Threading;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.Reactive;
using Directory = Zafiro.FileSystem.SeaweedFS.Filer.Client.Directory;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFileSystem : IZafiroFileSystem
{
    private readonly Maybe<ILogger> logger;
    private readonly ISeaweedFS seaweedFSClient;

    public SeaweedFileSystem(ISeaweedFS seaweedFSClient, Maybe<ILogger> logger)
    {
        this.seaweedFSClient = seaweedFSClient;
        this.logger = logger;
    }

    // TODO: Implement create file
    public Task<Result> CreateFile(ZafiroPath path) => throw new NotImplementedException();

    public IObservable<byte> GetFileContents(ZafiroPath path) => throw new NotSupportedException();

    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes, CancellationToken cancellationToken)
    {
        return Result.Try(async () =>
        {
            await using var stream = bytes.ToStream();
            await seaweedFSClient.Upload(ToServicePath(path), stream, cancellationToken);
        });
    }

    public Task<Result> CreateDirectory(ZafiroPath path) => Result.Try(() => seaweedFSClient.CreateFolder(ToServicePath(path)), e => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, e, logger));

    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path)
    {
        return Result
            .Try(() => seaweedFSClient.GetFileMetadata(ToServicePath(path), CancellationToken.None), e => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, e, logger))
            .Map(f => new FileProperties(false, f.Crtime, f.FileSize));
    }

    public async Task<Result<IDictionary<HashMethod, byte[]>>> GetHashes(ZafiroPath path) => await GetHashData(path);

    public async Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path) => Result
        .Success(new DirectoryProperties(false, DateTimeOffset.MinValue));

    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = default)
    {
        return Result.Try(() => seaweedFSClient.GetContents(ToServicePath(path), ct), e => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, e, logger))
            .Map(directory => directory.Entries?.OfType<FileMetadata>().Select(d => d.FullPath) ?? Enumerable.Empty<string>())
            .Map(x => x.Select(s => ToZafiroPath(s)));
    }

    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default)
    {
        return Result.Try(() => seaweedFSClient.GetContents(ToServicePath(path), ct), e => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, e, logger))
            .Map(directory => directory.Entries?.OfType<Directory>().Select(d => d.FullPath) ?? Enumerable.Empty<string>())
            .Map(x => x.Select(s => ToZafiroPath(s)));
    }

    public Task<Result<bool>> ExistDirectory(ZafiroPath path) => Result.Try(() => seaweedFSClient.PathExists(path + "/"), e => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, e, logger));

    public Task<Result<bool>> ExistFile(ZafiroPath path) => Result.Try(() => seaweedFSClient.PathExists(path), e => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, e, logger));

    public Task<Result> DeleteFile(ZafiroPath path) => Result.Try(() => seaweedFSClient.DeleteFile(ToServicePath(path)), e => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, e, logger));

    public Task<Result> DeleteDirectory(ZafiroPath path) => Result.Try(() => seaweedFSClient.DeleteFolder(ToServicePath(path)), e => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, e, logger));
    public Task<Result<Stream>> GetFileData(ZafiroPath path) => seaweedFSClient.GetFileContents(path);
    public Task<Result> SetFileData(ZafiroPath path, Stream stream, CancellationToken ct = default)
    {
        return Result.Try(async () =>
        {
            await seaweedFSClient.Upload(ToServicePath(path), stream, ct);
        });
    }

    private static string ToServicePath(ZafiroPath path)
    {
        if (path == "")
        {
            return "/";
        }

        return path;
    }

    private static ZafiroPath ToZafiroPath(ZafiroPath path) => path.ToString()[1..];

    private async Task<Result<IDictionary<HashMethod, byte[]>>> GetHashData(ZafiroPath path)
    {
        var result = await Result
            .Try(() => seaweedFSClient.GetFileMetadata(ToServicePath(path)))
            .Map(metadata => Maybe.From(metadata.Md5))
            .Map(maybeMd5 => maybeMd5.Map(s => (IDictionary<HashMethod, byte[]>) new Dictionary<HashMethod, byte[]>
            {
                [HashMethod.Md5] = Convert.FromBase64String(s!)
            }).GetValueOrDefault(new Dictionary<HashMethod, byte[]>()));

        return result;
    }
}