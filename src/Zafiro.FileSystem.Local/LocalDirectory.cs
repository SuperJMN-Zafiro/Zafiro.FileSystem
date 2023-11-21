using System.IO.Abstractions;
using Serilog;

namespace Zafiro.FileSystem.Local;

public class LocalDirectory : IZafiroDirectory
{
    private readonly IDirectoryInfo directoryInfo;
    private readonly Maybe<ILogger> logger;

    public LocalDirectory(IDirectoryInfo directoryInfo, Maybe<ILogger> logger, IFileSystem fileSystem)
    {
        this.directoryInfo = directoryInfo;
        this.logger = logger;
        FileSystem = fileSystem;
    }

    public bool IsHidden
    {
        get
        {
            if (directoryInfo.Root.FullName == directoryInfo.FullName)
            {
                return false;
            }

            if (directoryInfo.Attributes.HasFlag(FileAttributes.Hidden))
            {
                return true;
            }

            if (directoryInfo.Name.StartsWith("."))
            {
                return true;
            }

            return false;
        }
    }

    public ZafiroPath Path
    {
        get
        {
            if (directoryInfo.FullName.StartsWith("/"))
            {
                return directoryInfo.FullName[1..];
            }
            
            if (directoryInfo.FullName.EndsWith(System.IO.Path.DirectorySeparatorChar))
            {
                return directoryInfo.FullName[..^1].ToZafiroPath();
            }
            else
            {
                return directoryInfo.FullName.ToZafiroPath();
            }
        }
    }

    public IFileSystem FileSystem { get; }

    public async Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
    {
        var fromResult = await Task.FromResult(Result.Try(() => directoryInfo.GetDirectories().Select(info => (IZafiroDirectory) new LocalDirectory(info, logger, FileSystem)), ex => ExceptionHandler.HandleError(Path, ex, logger))).ConfigureAwait(false);
        return fromResult;
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles()
    {
        return Task.FromResult(Result.Try(() => directoryInfo.GetFiles().Select(info => (IZafiroFile) new LocalFile(info, logger)), ex => ExceptionHandler.HandleError(Path, ex, logger)));
    }

    public Task<Result> Delete()
    {
        return Task.FromResult(Result.Try(() => directoryInfo.Delete(true)));
    }

    public async Task<Result<IZafiroFile>> GetFile(string name)
    {
        if (name.Contains(ZafiroPath.ChunkSeparator))
        {
            return Result.Failure<IZafiroFile>("The name cannot be a path, but the name of a file in the directory");
        }

        return await FileSystem.GetFile(Path.Combine(name));
    }

    public override string ToString()
    {
        return Path;
    }
}