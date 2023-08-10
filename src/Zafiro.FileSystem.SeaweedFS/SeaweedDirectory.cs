using System.Net;
using CSharpFunctionalExtensions;
using Refit;
using Serilog;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Directory = Zafiro.FileSystem.SeaweedFS.Filer.Client.Directory;
using File = Zafiro.FileSystem.SeaweedFS.Filer.Client.File;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedDirectory : IZafiroDirectory
{
    private readonly SeaweedFSClient seaweedFS;
    private readonly Maybe<ILogger> logger;

    public SeaweedDirectory(ZafiroPath path, SeaweedFSClient seaweedFS, Maybe<ILogger> logger)
    {
        Path = path;
        this.seaweedFS = seaweedFS;
        this.logger = logger;
    }

    public ZafiroPath Path { get; }

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
    {
        return Result
            .Try(() => seaweedFS.CreateFolder(Path))
            .Bind(() => Result.Try(() => seaweedFS.GetContents(Path), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(Path, ex, logger)))
            .Map(GetDirectories);
    }

    private IEnumerable<IZafiroDirectory> GetDirectories(RootDirectory folder)
    {
        return folder.Entries?
            .OfType<Directory>()
            .Select(f => new SeaweedDirectory(f.FullPath[1..], seaweedFS, logger))  ?? Enumerable.Empty<IZafiroDirectory>();;
    }

    private IEnumerable<IZafiroFile> GetFiles(RootDirectory folder)
    {
        return folder.Entries?
            .OfType<File>()
            .Select(f => new SeaweedFile(f.FullPath[1..], seaweedFS, logger)) ?? Enumerable.Empty<IZafiroFile>();
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles()
    {
        return Result
            .Try(() => seaweedFS.CreateFolder(Path))
            .Bind(() => Result.Try(() => seaweedFS.GetContents(Path), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(Path, ex, logger)))
            .Map(GetFiles);
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath destPath)
    {
        return Task.FromResult(Result.Success((IZafiroFile)new SeaweedFile(destPath, seaweedFS, logger)));
    }

    public override string ToString()
    {
        return Path;
    }
}