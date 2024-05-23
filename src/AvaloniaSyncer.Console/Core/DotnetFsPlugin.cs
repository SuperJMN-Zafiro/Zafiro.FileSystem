using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;
using IDirectory = Zafiro.FileSystem.Lightweight.IDirectory;
using IFile = Zafiro.FileSystem.IFile;

namespace AvaloniaSyncer.Console.Plugins;

public class DotnetFsPlugin : IFileSystemPlugin
{
    private DotnetFsPlugin(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public IFileSystem FileSystem { get; }

    public string Name => "local";
    public string DisplayName => "Local Filesystem";

    public Task<Result<IDirectory>> GetFiles(ZafiroPath path)
    {
        return DotNetDirectory.From(path, new FileSystem())
            .Bind(x => x.ToLightweight());
    }

    public async Task<Result> Copy(IFile left, ZafiroPath destination)
    {
        using (var stream = FileSystem.File.Open(destination, FileMode.Create))
        {
            return await left.DumpTo(stream);
        }
    }

    public static async Task<Result<DotnetFsPlugin>> Create()
    {
        return Result.Try(() =>
        {
            var fs = new FileSystem();
            return new DotnetFsPlugin(fs);
        });
    }

    public override string ToString() => DisplayName;
}