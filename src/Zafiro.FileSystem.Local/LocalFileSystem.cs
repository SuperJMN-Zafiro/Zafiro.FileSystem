using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.FileSystem.Local;

public class LocalFileSystem : IFileSystem
{
    private readonly System.IO.Abstractions.IFileSystem fileSystem;
    private readonly Maybe<ILogger> logger;

    public LocalFileSystem(System.IO.Abstractions.IFileSystem fileSystem, Maybe<ILogger> logger)
    {
        this.fileSystem = fileSystem;
        this.logger = logger;
    }

    public Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroDirectory>(() =>
        {
            var directoryInfo = path.Path.EndsWith(":") ? fileSystem.DirectoryInfo.New(path.Path + "\\") : fileSystem.DirectoryInfo.New(path.Path);
            return new LocalDirectory(directoryInfo, logger, this);
        }, ex => ExceptionHandler.HandlePathAccessError(path, ex, logger)));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroFile>(() => new LocalFile(fileSystem.FileInfo.New(path), logger), ex => ExceptionHandler.HandlePathAccessError(path, ex, logger)));
    }

    public ZafiroPath GetRoot() => Directory.GetCurrentDirectory().ToZafiroPath();
}