using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;
using IDirectory = Zafiro.FileSystem.Lightweight.IDirectory;
using IFile = Zafiro.FileSystem.IFile;

namespace AvaloniaSyncer.Console.Core;

public class DotnetFs : ISyncFileSystem
{
    private DotnetFs(IFileSystem fileSystem)
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

    public static async Task<Result<DotnetFs>> Create()
    {
        return Result.Try(() =>
        {
            var fs = new FileSystem();
            return new DotnetFs(fs);
        });
    }

    public override string ToString() => DisplayName;
}