using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFileSystem : IFileSystem
{
    private readonly ISeaweedFS seaweedFSClient;
    private readonly Maybe<ILogger> logger;

    public SeaweedFileSystem(ISeaweedFS seaweedFSClient, Maybe<ILogger> logger)
    {
        this.seaweedFSClient = seaweedFSClient;
        this.logger = logger;
    }

    public Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path)
    {
        return Task.FromResult(Result.Success<IZafiroDirectory>(new SeaweedDirectory(path, seaweedFSClient, logger, this)));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return Task.FromResult(Result.Success<IZafiroFile>(new SeaweedFile(path, seaweedFSClient, logger)));
    }

    public ZafiroPath GetRoot()
    {
        return ZafiroPath.Empty;
    }
}