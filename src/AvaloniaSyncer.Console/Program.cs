// See https://aka.ms/new-console-template for more information

using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.Mixins;

Console.WriteLine("Hello, World!");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

await SeaweedFsPlugin.Create("http://192.168.1.29:8888")
    .Bind(x => x.GetFiles("Juegos/ROMs/3DS"))
    .Tap(files => Console.Write(files.JoinWithLines()))
    .TapError(Log.Error);

public class SeaweedFsPlugin
{
    private readonly ISeaweedFS seaweedFSClient;

    private SeaweedFsPlugin(ISeaweedFS seaweedFSClient)
    {
        this.seaweedFSClient = seaweedFSClient;
    }

    public string Name { get; set; }
    public string DisplayName { get; set; }

    public Task<Result<IEnumerable<IFile>>> GetFiles(ZafiroPath path)
    {
        return SeaweedFSDirectory.From(path, seaweedFSClient)
            .Bind(x => x.Children())
            .Map(x => x.OfType<IFile>());
    }

    public static async Task<Result<SeaweedFsPlugin>> Create(string https)
    {
        return Result.Try(() =>
        {
            var seaweedFSClient = new SeaweedFSClient(new HttpClient() { BaseAddress = new Uri(https) });
            return new SeaweedFsPlugin(seaweedFSClient);
        });
    }
}