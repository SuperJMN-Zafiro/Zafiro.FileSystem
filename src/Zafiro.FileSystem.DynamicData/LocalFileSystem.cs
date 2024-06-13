using System.IO.Abstractions;
using Zafiro.FileSystem.DynamicData;

namespace Zafiro.FileSystem.VNext;

public class LocalFileSystem(System.IO.Abstractions.IFileSystem fileSystem)
{
    public System.IO.Abstractions.IFileSystem FileSystem { get; } = fileSystem;

    public DynamicDirectory GetFolder(ZafiroPath path)
    {
        return new DynamicDirectory(FileSystem.DirectoryInfo.New("/" + path));
    }
}