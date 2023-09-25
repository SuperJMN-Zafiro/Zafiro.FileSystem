using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.FileSystem.Local;

public class LocalDirectory : IZafiroDirectory
{
    private readonly DirectoryInfo directoryInfo;
    private readonly Maybe<ILogger> logger;

    public LocalDirectory(DirectoryInfo directoryInfo, Maybe<ILogger> logger, IFileSystem fileSystem)
    {
        this.directoryInfo = directoryInfo;
        this.logger = logger;
        FileSystem = fileSystem;
    }

    public ZafiroPath Path => directoryInfo.FullName.ToZafiroPath();
    public IFileSystem FileSystem { get; }

    public async Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
    {
        var fromResult = await Task.FromResult(Result.Try(() => directoryInfo.GetDirectories().Select(info => (IZafiroDirectory) new LocalDirectory(info, logger, FileSystem)), ex => ExceptionHandler.HandlePathAccessError(Path, ex, logger))).ConfigureAwait(false);
        return fromResult;
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles()
    {
        return Task.FromResult(Result.Try(() => directoryInfo.GetFiles().Select(info => (IZafiroFile) new LocalFile(info, logger)), ex => ExceptionHandler.HandlePathAccessError(Path, ex, logger)));
    }

    public Task<Result<IZafiroFile>> GetFile(string file)
    {
        return Task.FromResult(Result.Try(() => (IZafiroFile)new LocalFile(new FileInfo(Path.Combine(file)), logger), ex => ExceptionHandler.HandlePathAccessError(file, ex, logger)));
    }

    public override string ToString()
    {
        return Path;
    }
}