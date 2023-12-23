using System.Reactive.Linq;
using System.Text;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.IO;
using Zafiro.Mixins;
using Directory = Zafiro.FileSystem.SeaweedFS.Filer.Client.Directory;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFileSystem : IZafiroFileSystem
{
    private readonly ISeaweedFS seaweedFSClient;
    private readonly Maybe<ILogger> logger;

    public SeaweedFileSystem(ISeaweedFS seaweedFSClient, Maybe<ILogger> logger)
    {
        this.seaweedFSClient = seaweedFSClient;
        this.logger = logger;
    }

    // TODO: Implement create file
    public Task<Result> CreateFile(ZafiroPath path) => throw new NotImplementedException();

    public IObservable<byte> GetFileContents(ZafiroPath path)
    {
        return ObservableFactory.UsingAsync(() => seaweedFSClient.GetFileContent(ToServicePath(path)), stream => stream.ToObservable());
    }

    private static string ToServicePath(ZafiroPath path)
    {
        if (path == "")
        {
            return "/";
        }
        return path;
    }

    private static ZafiroPath ToZafiroPath(ZafiroPath path)
    {
        return path.ToString()[1..];
    }

    public async Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes)
    {
        return await Observable.Using(() => bytes.ToStream(), stream => Observable.FromAsync(ct => Result.Try(() => seaweedFSClient.Upload(ToServicePath(path), stream, ct))));
    }

    public Task<Result> CreateDirectory(ZafiroPath path) => Result.Try(() => seaweedFSClient.CreateFolder(ToServicePath(path)), e => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, e, logger));

    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path)
    {
        return Result
            .Try(() => seaweedFSClient.GetFileMetadata(ToServicePath(path), CancellationToken.None), e => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, e, logger))
            .Map(f => new FileProperties(false, f.Crtime, f.FileSize, GetChecksumData(f)));
    }

    private static Dictionary<ChecksumKind, byte[]> GetChecksumData(FileMetadata f)
    {
        var bytesMap = Maybe.From(f.Md5).Map(s => new Dictionary<ChecksumKind, byte[]>()
        {
            [ChecksumKind.Md5] = Encoding.UTF8.GetBytes(s!),
        }).GetValueOrDefault(new Dictionary<ChecksumKind, byte[]>());
        return bytesMap;
    }

    public async Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path)
    {
        return Result
            .Success(new DirectoryProperties(false, DateTimeOffset.MinValue));
    }

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
}