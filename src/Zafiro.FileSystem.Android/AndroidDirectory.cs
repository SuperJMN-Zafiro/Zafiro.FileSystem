using System.IO.Abstractions;
using Serilog;

namespace Zafiro.FileSystem.Android;

public class AndroidDirectory : IZafiroDirectory
{
    private readonly IDirectoryInfo directoryInfo;
    private readonly Maybe<ILogger> logger;

    public AndroidDirectory(IDirectoryInfo directoryInfo, Maybe<ILogger> logger, IFileSystem fileSystem)
    {
        this.directoryInfo = directoryInfo;
        this.logger = logger;
        FileSystem = fileSystem;
    }

    public ZafiroPath Path
    {
        get
        {
            if (directoryInfo.FullName.EndsWith(System.IO.Path.DirectorySeparatorChar))
            {
                return directoryInfo.FullName[..^1].FromAndroidToZafiro();
            }
            else
            {
                return directoryInfo.FullName[1..];
            }
        }
    }

    public IFileSystem FileSystem { get; }

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
    {
        return AndroidPermissions.Request().Bind(async () =>
        {
            var fromResult = await Task.FromResult(Result.Try(() => directoryInfo.GetDirectories()
                .Select(info => (IZafiroDirectory) new AndroidDirectory(info, logger, FileSystem)), ex => ExceptionHandler.HandleError(Path, ex, logger))).ConfigureAwait(false);
            return fromResult;
        });
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles()
    {
        return AndroidPermissions.Request().Bind(() => Result.Try(() => directoryInfo.GetFiles().Select(info => (IZafiroFile) new AndroidFile(info, logger)), ex => ExceptionHandler.HandleError(Path, ex, logger)));
    }

    public Task<Result> Delete()
    {
        return AndroidPermissions.Request().Bind(() => Result.Try(() => directoryInfo.Delete(true)));
    }

    public async Task<Result<IZafiroFile>> GetFile(string name)
    {
        if (name.Contains(ZafiroPath.ChunkSeparator))
        {
            return Result.Failure<IZafiroFile>("GetFile is used to get direct files in the directory. Subpaths are not supported. Use FileSystem.GetFile instead.");
        }

        return await FileSystem.GetFile(Path.Combine(name));
    }

    public override string ToString()
    {
        return Path;
    }
}