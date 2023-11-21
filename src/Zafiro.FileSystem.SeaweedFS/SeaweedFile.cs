using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFile : IZafiroFile
{
    private readonly ISeaweedFS seaweedStore;
    private readonly Maybe<ILogger> logger;

    public SeaweedFile(ZafiroPath path, ISeaweedFS seaweedStore, Maybe<ILogger> logger)
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

    public Task<Result<bool>> Exists()
    {
        return Result.Try(() => seaweedStore.PathExists(Path));
    }

    public Task<Result<Stream>> GetContents(CancellationToken cancellationToken = default)
    {
        return Result.Try(() => seaweedStore.GetFileContent(Path, cancellationToken), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(Path, ex, logger));
    }

    public Task<Result> SetContents(Stream stream, CancellationToken cancellationToken)
    {
        return Result.Try(() => seaweedStore.Upload(Path, stream, cancellationToken), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(Path, ex, logger));
    }

    public Task<Result> Delete(CancellationToken cancellationToken = default)
    {
        return Result.Try(() => seaweedStore.DeleteFile(Path, cancellationToken), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(Path, ex, logger));
    }

    public bool IsHidden => false;

    public override string ToString()
    {
        return Path;
    }
}