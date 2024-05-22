using System.IO.Abstractions;
using Zafiro.FileSystem.Lightweight;
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
        var fileNodes = DirectoryInfo.GetFiles().Select(info => (INode)new DotNetFile(info)).Concat(DirectoryInfo.GetDirectories().Select(x => (INode)new DotNetDirectory(x)));
        return Result.Success(fileNodes);
    }

    public static async Task<Result<IAsyncDir>> From(ZafiroPath path)
    {
        return Result.Success((IAsyncDir)new DotNetDirectory(new System.IO.Abstractions.FileSystem().DirectoryInfo.New(path)));
    }

    public string Name => DirectoryInfo.Name;
}