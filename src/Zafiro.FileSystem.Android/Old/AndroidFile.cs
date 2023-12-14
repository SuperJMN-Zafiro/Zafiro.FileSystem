using System.IO.Abstractions;
using Serilog;

namespace Zafiro.FileSystem.Android.Old;

public class AndroidFile : IZafiroFile
{
    private readonly IFileInfo info;
    private readonly Maybe<ILogger> logger;

    public AndroidFile(IFileInfo info, Maybe<ILogger> logger)
    {
        this.info = info;
        this.logger = logger;
    }

    public ZafiroPath Path => info.FullName.FromAndroidToZafiro();

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
            EnsureFileExists();
            return (Stream)info.OpenRead();
        }, ex => ExceptionHandler.HandleError(Path, ex, logger)));
    }

    public Task<Result> SetContents(Stream stream, CancellationToken cancellationToken)
    {
        return Result.Try(async () =>
        {
            EnsureFileExists();
            var fileStream = info.Open(FileMode.Truncate, FileAccess.Write);
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

    public bool IsHidden => false;

    public override string ToString()
    {
        return Path;
    }

    private void EnsureFileExists()
    {
        if (info.Directory != null)
        {
            var dir = info.Directory;
            if (!dir.Exists)
            {
                dir.Create();
            }
        }

        if (!info.Exists)
        {
            using (info.Create())
            {
            }
        }
    }
}