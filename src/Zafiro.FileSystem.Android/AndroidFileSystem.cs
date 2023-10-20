using Serilog;

namespace Zafiro.FileSystem.Android;

public class AndroidFileSystem : IFileSystem
{
    private readonly System.IO.Abstractions.IFileSystem fileSystem;
    private readonly Maybe<ILogger> logger;

    public AndroidFileSystem(System.IO.Abstractions.IFileSystem fileSystem, Maybe<ILogger> logger)
    {
        this.fileSystem = fileSystem;
        this.logger = logger;
    }

    public Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroDirectory>(() =>
        {
            var localPath = "/" + path;
            var directoryInfo = fileSystem.DirectoryInfo.New(localPath);
            return new AndroidDirectory(directoryInfo, logger, this);
        }, ex => ExceptionHandler.HandleError(path, ex, logger)));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroFile>(() => new AndroidFile(fileSystem.FileInfo.New(path), logger), ex => ExceptionHandler.HandleError(path, ex, logger)));
    }

    public Task<Result<ZafiroPath>> GetRoot()
    {
#if ANDROID
        return AndroidPermissions.Request().Bind(() =>
        {
            var path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            return ZafiroPath.Create(path[1..]);
        });
#endif
        return Task.FromResult(Result.Failure<ZafiroPath>("Not supported"));
    }
}