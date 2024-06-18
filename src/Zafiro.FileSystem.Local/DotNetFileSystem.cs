using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Zafiro.FileSystem.Local;

public class DotNetFileSystem : DynamicData.IFileSystem
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