using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Lightweight;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.FileSystem.VNext.Tests;

namespace AvaloniaSyncer.Console;

public class FileSystemPlugin : IFileSystemPlugin
{
    private readonly ISeaweedFS seaweedFSClient;

    private FileSystemPlugin(ISeaweedFS seaweedFSClient)
    {
        this.seaweedFSClient = seaweedFSClient;
    }

    public string Name { get; set; }
    public string DisplayName { get; set; }

    public Task<Result<IDirectory>> GetFiles(ZafiroPath path)
    {
        return SeaweedFSDirectory.From(path, seaweedFSClient)
            .Bind(x => x.ToLightweight());
    }

    public static async Task<Result<FileSystemPlugin>> Create(string https)
    {
        return Result.Try(() =>
        {
            var seaweedFSClient = new SeaweedFSClient(new HttpClient() { BaseAddress = new Uri(https) });
            return new FileSystemPlugin(seaweedFSClient);
        });
    }
}