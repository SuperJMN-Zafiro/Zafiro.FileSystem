using Zafiro.FileSystem.DynamicData;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Zafiro.FileSystem.Local.Mutable;

public class DotNetFileSystem : Zafiro.FileSystem.DynamicData.IFileSystem
{
    public IFileSystem FileSystem { get; }

    public DotNetFileSystem(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public async Task<Result<IRooted<IDynamicDirectory>>> Get(ZafiroPath path)
    {
        return Result
            .Try(() => FileSystem.DirectoryInfo.New("/" + path))
            .Map(d => new LocalDynamicDirectory(d))
            .Map(directory => (IRooted<IDynamicDirectory>)new Rooted<IDynamicDirectory>(path, directory));
    }
}