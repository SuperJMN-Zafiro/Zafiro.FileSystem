using System.IO.Abstractions;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Lightweight;

public class DotnetDir : IDirectory
{
    private readonly IDirectoryInfo directoryInfo;

    public DotnetDir(IDirectoryInfo directoryInfo)
    {
        this.directoryInfo = directoryInfo;
        Name = directoryInfo.Name;
        Children = directoryInfo.GetFileSystemInfos().Select(Create);
    }

    private INode Create(IFileSystemInfo info)
    {
        return info switch
        {
            IDirectoryInfo di => new DotnetDir(di),
            IFileInfo fi => new DotNetFile(fi),
            _ => throw new ArgumentOutOfRangeException(nameof(info))
        };
    }

    public string Name { get; }
    public IEnumerable<INode> Children { get; }
    public override string ToString() => directoryInfo.ToString() ?? "";
}