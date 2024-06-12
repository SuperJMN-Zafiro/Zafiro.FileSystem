using System.IO.Abstractions;
using Zafiro.FileSystem.DynamicData;

namespace Zafiro.FileSystem.VNext;

public class LocalFileSystem(IFileSystem fileSystem)
{
    public IFileSystem FileSystem { get; } = fileSystem;

    public DynamicDirectory GetFolder(ZafiroPath path)
    {
        return new DynamicDirectory(FileSystem.DirectoryInfo.New("/" + path));
    }
}