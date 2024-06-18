namespace Zafiro.FileSystem.Local;

public class LocalFileSystem(System.IO.Abstractions.IFileSystem fileSystem)
{
    public System.IO.Abstractions.IFileSystem FileSystem { get; } = fileSystem;

    public LocalDynamicDirectory GetFolder(ZafiroPath path)
    {
        return new LocalDynamicDirectory(FileSystem.DirectoryInfo.New("/" + path));
    }
}