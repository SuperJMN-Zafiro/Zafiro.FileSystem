using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using ClientDirectory = Zafiro.FileSystem.SeaweedFS.Filer.Client.Directory;
using ILogger = Serilog.ILogger;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFSDirectory : IAsyncDir
{
    private SeaweedFSDirectory(string path, ISeaweedFS seaweedFS)
    {
        SeaweedFS = seaweedFS;
        Path = path;
    }

    public string Path { get; }
    public ISeaweedFS SeaweedFS { get; }
    public string Name => Path[Path.LastIndexOf("/")..];

    public Task<Result<IEnumerable<INode>>> Children()
    {
        return SeaweedFS.GetContents(Path, CancellationToken.None).Bind(GetContents);
    }

    public static async Task<Result<SeaweedFSDirectory>> From(string path, ISeaweedFS seaweedFSClient)
    {
        return new SeaweedFSDirectory(path, seaweedFSClient);
    }

    private Task<Result<IEnumerable<INode>>> GetContents(RootDirectory directory)
    {
        var entries = directory.Entries ?? new List<BaseEntry>();
        return entries.Select(entry =>
        {
            return entry switch
            {
                ClientDirectory dir => Task.FromResult(Result.Success<INode>(new SeaweedFSDirectory(dir.FullPath[1..], SeaweedFS))),
                FileMetadata file => SeaweedFSFile.From(file.FullPath[1..], SeaweedFS).Map(fsFile => (INode)fsFile),
                _ => throw new ArgumentOutOfRangeException(nameof(entry))
            };
        }).Combine();
    }
}