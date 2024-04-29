namespace Zafiro.FileSystem.Lightweight;

public class RootedFile : IRootedFile
{
    public RootedFile(ZafiroPath path, IFile file)
    {
        Path = path;
        File = file;
    }

    public IFile File { get; }
    public ZafiroPath Path { get; }

    public IFile Rooted => File;
    public string Name => File.Name;
    public IObservable<byte[]> Bytes => File.Bytes;
    public long Length => File.Length;

    public override string ToString() => this.FullPath();
}