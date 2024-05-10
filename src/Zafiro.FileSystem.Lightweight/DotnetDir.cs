using System.IO.Abstractions;

namespace Zafiro.FileSystem.Lightweight;

public class DotnetDir : ISlimDirectory
{
    private readonly IDirectoryInfo directoryInfo;

    public DotnetDir(IDirectoryInfo directoryInfo)
    {
        this.directoryInfo = directoryInfo;
        Name = directoryInfo.Name;
        Children = directoryInfo.GetFileSystemInfos().Select(info => Create(info));
    }

    private INode Create(IFileSystemInfo info)
    {
        return info switch
        {
            IDirectoryInfo directoryInfo => new DotnetDir(directoryInfo),
            IFileInfo fileInfo => new DotnetFile(fileInfo),
            _ => throw new ArgumentOutOfRangeException(nameof(info))
        };
    }

    public string Name { get; }
    public IEnumerable<INode> Children { get; }
    public override string ToString() => directoryInfo.ToString() ?? "";
}