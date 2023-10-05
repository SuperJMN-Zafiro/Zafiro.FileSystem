using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.FileSystem.Local;

public class LocalFile : IZafiroFile
{
    private readonly FileInfo info;
    private readonly Maybe<ILogger> logger;

    public LocalFile(FileInfo info, Maybe<ILogger> logger)
    {
        this.info = info;
        this.logger = logger;
    }

    public ZafiroPath Path => info.FullName.ToZafiroPath();

    public Task<Result<long>> Size()
    {
        return Result.Try(() => Task.FromResult(info.Length));
    }

    public Task<Result<bool>> Exists()
    {
        return Task.FromResult<Result<bool>>(info.Exists);
    }

    public Task<Result<Stream>> GetContents(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Try(() =>
        {
            EnsureFileExists(Path.FromZafiroPath());
            return (Stream) File.OpenRead(Path);
        }, ex => ExceptionHandler.HandlePathAccessError(Path, ex, logger)));
    }

    public Task<Result> SetContents(Stream stream, CancellationToken cancellationToken)
    {
        return Result.Try(async () =>
        {
            EnsureFileExists(Path.FromZafiroPath());
            var fileStream = File.OpenWrite(Path);
            await using var _ = fileStream.ConfigureAwait(false);
            {
                await stream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
            }
        });
    }

    public Task<Result> Delete(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Try(() => info.Delete()));
    }

    public override string ToString()
    {
        return Path;
    }

    private void EnsureFileExists(string path)
    {
        var directoryName = System.IO.Path.GetDirectoryName(path);

        if (directoryName != null && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        if (!File.Exists(path))
        {
            using (File.Create(path))
            {
            }
        }
    }
}