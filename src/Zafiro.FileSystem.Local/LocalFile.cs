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

    public Task<Result<long>> Size() => Result.Try(() => Task.FromResult(info.Length));

    public Task<Result<Stream>> GetContents()
    {
        return Task.FromResult(Result.Try(() =>
        {
            EnsureFileExists(Path.FromZafiroPath());
            return (Stream)new DisposeAwareStream(Path, File.OpenRead(Path), logger);
        }, ex => ExceptionHandler.HandlePathAccessError(Path, ex, logger)));
    }

    public Task<Result> SetContents(Stream stream, CancellationToken cancellationToken)
    {
        return Result.Try(async () =>
        {
            EnsureFileExists(Path.FromZafiroPath());
            await using (var fileStream = new DisposeAwareStream(Path, File.OpenWrite(Path), logger))
            {
                {
                    await stream.CopyToAsync(fileStream, cancellationToken);
                }
                await fileStream.DisposeAsync();
            }
        });
    }

    public Task<Result> Delete()
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
            using (File.Create(path)) { }
        }
    }
}