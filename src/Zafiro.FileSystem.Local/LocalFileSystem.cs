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
        if (path == ZafiroPath.Empty)
        {
            if (OperatingSystem.IsWindows())
            {
                return Task.FromResult(Result.Success((IZafiroDirectory)new RootDirectory(fileSystem, this, logger)));
            }

            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                var directoryInfo = fileSystem.DirectoryInfo.New("/");
                return Task.FromResult(Result.Try(() => (IZafiroDirectory)new LocalDirectory(directoryInfo, logger, this)));
            }
        }

        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            return Task.FromResult(Result.Try(() => fileSystem.DirectoryInfo.New("/" + path))
                .Map(info => (IZafiroDirectory)new LocalDirectory(info, logger, this)));
        }
        
        return Task.FromResult(Result.Try<IZafiroDirectory>(() =>
        {
            
            
            var directoryInfo = path.Path.EndsWith(":") ? fileSystem.DirectoryInfo.New(path.Path + "\\") : fileSystem.DirectoryInfo.New(path.Path);
            return new LocalDirectory(directoryInfo, logger, this);
        }, ex => ExceptionHandler.HandleError(path, ex, logger)));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroFile>(() => new LocalFile(fileSystem.FileInfo.New(path), logger), ex => ExceptionHandler.HandleError(path, ex, logger)));
    }

    public Task<Result<ZafiroPath>> GetRoot() => Task.FromResult<Result<ZafiroPath>>(Directory.GetCurrentDirectory().ToZafiroPath());
}