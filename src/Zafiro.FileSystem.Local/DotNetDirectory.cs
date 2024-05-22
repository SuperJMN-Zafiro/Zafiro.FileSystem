using System.IO.Abstractions;
using Zafiro.FileSystem.VNext.Interfaces;

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
        var files = DirectoryInfo.GetFiles().Select(info => (INode)new DotNetFile(info));
        var dirs = DirectoryInfo.GetDirectories().Select(x => (INode)new DotNetDirectory(x));
        var nodes = files.Concat(dirs);
        return Result.Success(nodes);
    }

    public static async Task<Result<IAsyncDir>> From(ZafiroPath path, IFileSystem fileSystem)
    {
        return Result.Success((IAsyncDir)new DotNetDirectory(fileSystem.DirectoryInfo.New(path)));
    }

    public string Name => DirectoryInfo.Name;
}