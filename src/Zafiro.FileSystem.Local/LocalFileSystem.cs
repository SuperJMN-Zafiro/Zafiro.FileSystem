using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.FileSystem.Local;

public class LocalFileSystem : IFileSystem
{
    private readonly Maybe<ILogger> logger;

    public LocalFileSystem(Maybe<ILogger> logger)
    {
        this.logger = logger;
    }

    public Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroDirectory>(() => new LocalDirectory(new DirectoryInfo(path), logger, this), ex => ExceptionHandler.HandlePathAccessError(path, ex, logger)));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroFile>(() => new LocalFile(new FileInfo(path), logger), ex => ExceptionHandler.HandlePathAccessError(path, ex, logger)));
    }
}