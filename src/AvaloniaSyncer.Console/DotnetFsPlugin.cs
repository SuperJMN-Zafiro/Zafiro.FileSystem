using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;
using Zafiro.FileSystem.VNext.Tests;
using IDirectory = Zafiro.FileSystem.Lightweight.IDirectory;

namespace AvaloniaSyncer.Console;

public class DotnetFsPlugin : IFileSystemPlugin
{
    public IFileSystem FileSystem { get; }

    private DotnetFsPlugin(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public string Name { get; set; }
    public string DisplayName { get; set; }

    public Task<Result<IDirectory>> GetFiles(ZafiroPath path)
    {
        return DotNetDirectory.From(path, new FileSystem())
            .Bind(x => x.ToLightweight());
    }

    public static async Task<Result<DotnetFsPlugin>> Create()
    {
        return Result.Try(() =>
        {
            var fs = new FileSystem();
            return new DotnetFsPlugin(fs);
        });
    }
}