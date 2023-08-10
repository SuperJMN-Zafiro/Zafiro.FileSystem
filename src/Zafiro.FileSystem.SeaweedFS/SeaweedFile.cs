using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFile : IZafiroFile
{
    private readonly SeaweedFSClient seaweedStore;
    private readonly Maybe<ILogger> logger;

    public SeaweedFile(ZafiroPath path, SeaweedFSClient seaweedStore, Maybe<ILogger> logger)
    {
        Path = path;
        this.seaweedStore = seaweedStore;
        this.logger = logger;
    }

    public ZafiroPath Path { get; }

    public Task<Result<long>> Size() => Result
        .Try(() => seaweedStore.GetFileMetadata(Path))
        .Map(file => file.FileSize)
        .MapError(s => $"Cannot retrieve file size for '{Path}'. Reason: {s}");

    public Task<Result<Stream>> GetContents()
    {
        return Result.Try(() => seaweedStore.GetFileContent(Path), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(Path, ex, logger));
    }

    public Task<Result> SetContents(Stream stream)
    {
        return Result.Try(() => seaweedStore.Upload(Path, stream), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(Path, ex, logger));
    }

    public Task<Result> Delete()
    {
        return Result.Try(() => seaweedStore.DeleteFile(Path), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(Path, ex, logger));
    }

    public override string ToString()
    {
        return Path;
    }
}