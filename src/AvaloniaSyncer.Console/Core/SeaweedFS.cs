using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Lightweight;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace AvaloniaSyncer.Console;

public class SeaweedFS : ISyncFileSystem
{
    private readonly ISeaweedFS seaweedFSClient;

    private SeaweedFS(ISeaweedFS seaweedFSClient)
    {
        this.seaweedFSClient = seaweedFSClient;
    }

    public string Name => "seaweedfs";
    public string DisplayName => "SeaweedFS";

    public Task<Result<IDirectory>> GetFiles(ZafiroPath path)
    {
        return SeaweedFSDirectory.From(path, seaweedFSClient)
            .Bind(x => x.ToLightweight());
    }

    public Task<Result> Copy(IFile left, ZafiroPath destinationFileName)
    {
        return Result.Try(async () =>
        {
            using var stream = left.ToStream();
            await seaweedFSClient.Upload(destinationFileName, stream);
        });
    }

    public static async Task<Result<SeaweedFS>> Create(string https)
    {
        return Result.Try(() =>
        {
            var seaweedFSClient = new SeaweedFSClient(new HttpClient() { BaseAddress = new Uri(https) });
            return new SeaweedFS(seaweedFSClient);
        });
    }

    public override string ToString() => DisplayName;
}