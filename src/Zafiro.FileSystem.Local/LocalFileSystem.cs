namespace Zafiro.FileSystem.Local;

public class LocalFileSystem(IFileSystem fileSystem)
{
    public IFileSystem FileSystem { get; } = fileSystem;

    public LocalDynamicDirectory GetFolder(ZafiroPath path)
    {
        return new LocalDynamicDirectory(FileSystem.DirectoryInfo.New("/" + path));
    }
}