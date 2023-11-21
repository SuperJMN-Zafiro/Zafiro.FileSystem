using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Directory = Zafiro.FileSystem.SeaweedFS.Filer.Client.Directory;
using File = Zafiro.FileSystem.SeaweedFS.Filer.Client.File;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedDirectory : IZafiroDirectory
{
    private readonly ISeaweedFS seaweedFS;
    private readonly Maybe<ILogger> logger;

    public SeaweedDirectory(ZafiroPath path, ISeaweedFS seaweedFS, Maybe<ILogger> logger, IFileSystem fileSystem)
    {
        Path = path;
        this.seaweedFS = seaweedFS;
        this.logger = logger;
        FileSystem = fileSystem;
    }

    public bool IsHidden => false;
    public ZafiroPath Path { get; }
    public IFileSystem FileSystem { get; }

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
    {
        return Result
            .Try(() => seaweedFS.CreateFolder(Path.ToServicePath()), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(Path, ex, logger))
            .Bind(() => Result.Try(() => seaweedFS.GetContents(Path.ToServicePath()), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(Path, ex, logger)))
            .Map(GetDirectories);
    }

    private IEnumerable<IZafiroDirectory> GetDirectories(RootDirectory folder)
    {
        return folder.Entries?
            .OfType<Directory>()
            .Select(f => new SeaweedDirectory(f.FullPath.ToZafiroPath(), seaweedFS, logger, FileSystem))  ?? Enumerable.Empty<IZafiroDirectory>();
    }

    private IEnumerable<IZafiroFile> GetFiles(RootDirectory folder)
    {
        return folder.Entries?
            .OfType<File>()
            .Select(f => new SeaweedFile(f.FullPath.ToZafiroPath(), seaweedFS, logger)) ?? Enumerable.Empty<IZafiroFile>();
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles()
    {
        return Result
            .Try(() => seaweedFS.CreateFolder(Path.ToServicePath()))
            .Bind(() => Result.Try(() => seaweedFS.GetContents(Path.ToServicePath()), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(Path, ex, logger)))
            .Map(GetFiles);
    }

    public Task<Result> Delete()
    {
        return Result
            .Try(() => seaweedFS.DeleteFolder(Path));
    }

    public Task<Result<IZafiroFile>> GetFile(string destPath)
    {
        return Task.FromResult(Result.Success((IZafiroFile)new SeaweedFile(Path.Combine(destPath), seaweedFS, logger)));
    }

    public override string ToString()
    {
        return Path;
    }
}