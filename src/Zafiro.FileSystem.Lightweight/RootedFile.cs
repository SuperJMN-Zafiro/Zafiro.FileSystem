namespace Zafiro.FileSystem.Lightweight;

public class RootedFile
{
    public RootedFile(ZafiroPath path, IFile file)
    {
        Path = path;
        File = file;
    }

    public ZafiroPath Path { get; }
    public IFile File { get; }
}