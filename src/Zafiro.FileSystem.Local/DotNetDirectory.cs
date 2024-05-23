using System.IO.Abstractions;

namespace Zafiro.FileSystem.Local;

public class DotNetDirectory : IAsyncDir
{
    public IDirectoryInfo DirectoryInfo { get; }

    public DotNetDirectory(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
    }

    public async Task<Result<IEnumerable<INode>>> Children()
    {
        return Result.Try(() =>
        {
            var files = DirectoryInfo.GetFiles().Select(info => (INode) new DotNetFile(info));
            var dirs = DirectoryInfo.GetDirectories().Select(x => (INode) new DotNetDirectory(x));
            var nodes = files.Concat(dirs);
            return nodes;
        });
    }

    public static async Task<Result<IAsyncDir>> From(ZafiroPath path, IFileSystem fileSystem)
    {
        return Result.Try(() =>
        {
            var directoryInfo = fileSystem.DirectoryInfo.New(path);
            return (IAsyncDir) new DotNetDirectory(directoryInfo);
        });
    }

    public string Name => DirectoryInfo.Name;
}