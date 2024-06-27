using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using ClientDirectory = Zafiro.FileSystem.SeaweedFS.Filer.Client.Directory;
using ILogger = Serilog.ILogger;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFSDirectory : IAsyncDir
{
    public SeaweedFSDirectory(ClientDirectory dir, ISeaweedFS api)
    {
        Dir = dir;
        Api = api;
        Path = (ZafiroPath)(dir.FullPath.StartsWith("/") ? dir.FullPath[1..] : dir.FullPath);
    }

    public string QueryablePath => Path == ZafiroPath.Empty ? "/" : Path;
    public ZafiroPath Path { get; }
    public ClientDirectory Dir { get; }
    public ISeaweedFS Api { get; }
    public string Name => Path.Name();

    public Task<Result<IEnumerable<INode>>> Children() => Result
        .Try(
            () => Api.GetContents(QueryablePath, CancellationToken.None), 
            e => RefitBasedAccessExceptionHandler.HandlePathAccessError(QueryablePath, e, Maybe<ILogger>.None))
        .Map(GetContents);

    public static Task<Result<IAsyncDir>> From(ZafiroPath path, ISeaweedFS seaweedFSClient)
    {
        return Result
            .Try(() => seaweedFSClient.GetContents(path, CancellationToken.None), e => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, e, Maybe<ILogger>.None))
            .Map(directory =>
            {
                var dir = new ClientDirectory()
                {
                    FullPath = directory.Path
                };
                
                return (IAsyncDir) new SeaweedFSDirectory(dir, seaweedFSClient);
            });
    }

    private IEnumerable<INode> GetContents(RootDirectory directory)
    {
        foreach (var directoryEntry in directory.Entries ?? new List<BaseEntry>())
        {
            yield return directoryEntry switch
            {
                ClientDirectory dir => new SeaweedFSDirectory(dir, Api),
                FileMetadata file => new SeaweedFSFile(file, Api),
                _ => throw new ArgumentOutOfRangeException(nameof(directoryEntry))
            };
        }
    }
}