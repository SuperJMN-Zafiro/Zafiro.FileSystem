using Serilog;

namespace Zafiro.FileSystem.Local;

public class RootDirectory : IZafiroDirectory
{
    private readonly System.IO.Abstractions.IFileSystem fileSystem;
    private readonly LocalFileSystem localFileSystem;
    private readonly Maybe<ILogger> logger;

    public RootDirectory(System.IO.Abstractions.IFileSystem fileSystem, LocalFileSystem localFileSystem, Maybe<ILogger> logger)
    {
        this.fileSystem = fileSystem;
        this.localFileSystem = localFileSystem;
        this.logger = logger;
    }

    public ZafiroPath Path => ZafiroPath.Empty;
    public IFileSystem FileSystem => localFileSystem;
    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
    {
        var result = Task.FromResult(Result
            .Try(() => fileSystem.DriveInfo.GetDrives())
            .Map(drives => drives.Select(info => (IZafiroDirectory)new LocalDirectory(info.RootDirectory, logger, localFileSystem))));

        return result;
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles() => Task.FromResult(Result.Success(Enumerable.Empty<IZafiroFile>()));

    public Task<Result> Delete() => Task.FromResult(Result.Failure("Cannot delete root"));
}