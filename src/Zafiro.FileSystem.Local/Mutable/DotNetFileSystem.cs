using Zafiro.FileSystem.DynamicData;
using Zafiro.FileSystem.Mutable;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Zafiro.FileSystem.Local.Mutable;

public class DotNetFileSystem : Zafiro.FileSystem.DynamicData.IFileSystem
{
    public IFileSystem FileSystem { get; }

    public DotNetFileSystem(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public Task<Result<IRooted<IMutableDirectory>>> Get(ZafiroPath path)
    {
        return Task.FromResult(Result
            .Try(() => FileSystem.DirectoryInfo.New("/" + path))
            .Map(d => new DotNetMutableDirectory(new DotNetDirectory(d)))
            .Map(directory => (IRooted<IMutableDirectory>)new Rooted<IMutableDirectory>(path, directory)));
    }
}