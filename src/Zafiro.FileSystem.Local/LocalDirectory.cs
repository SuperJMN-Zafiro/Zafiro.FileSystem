using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.FileSystem.Local;

public class LocalDirectory : IZafiroDirectory
{
    private readonly DirectoryInfo directoryInfo;
    private readonly Maybe<ILogger> logger;

    public LocalDirectory(DirectoryInfo directoryInfo, Maybe<ILogger> logger)
    {
        this.directoryInfo = directoryInfo;
        this.logger = logger;
    }

    public ZafiroPath Path => directoryInfo.FullName.ToZafiroPath();

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
    {
        return Task.FromResult(Result.Try(() => directoryInfo.GetDirectories().Select(info => (IZafiroDirectory) new LocalDirectory(info, logger)), ex => ExceptionHandler.HandlePathAccessError(Path, ex, logger)));
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles()
    {
        return Task.FromResult(Result.Try(() => directoryInfo.GetFiles().Select(info => (IZafiroFile) new LocalFile(info, logger)), ex => ExceptionHandler.HandlePathAccessError(Path, ex, logger)));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath destPath)
    {
        return Task.FromResult(Result.Try(() => (IZafiroFile)new LocalFile(new FileInfo(destPath), logger), ex => ExceptionHandler.HandlePathAccessError(destPath, ex, logger)));
    }

    public override string ToString()
    {
        return Path;
    }
}