using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFileSystem : IFileSystem
{
    private readonly SeaweedFSClient seaweedFSClient;
    private readonly Maybe<ILogger> logger;

    public SeaweedFileSystem(SeaweedFSClient seaweedFSClient, Maybe<ILogger> logger)
    {
        this.seaweedFSClient = seaweedFSClient;
        this.logger = logger;
    }

    public Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path)
    {
        return Task.FromResult(Result.Success<IZafiroDirectory>(new SeaweedDirectory(path, seaweedFSClient, logger)));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return Task.FromResult(Result.Success<IZafiroFile>(new SeaweedFile(path, seaweedFSClient, logger)));
    }
}