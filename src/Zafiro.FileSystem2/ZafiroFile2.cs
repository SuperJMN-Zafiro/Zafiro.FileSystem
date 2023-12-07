using Zafiro.FileSystem;

namespace Zafiro.FileSystem2;

public class ZafiroFile2 : IZafiroFile2
{
    private readonly ZafiroPath path;
    private readonly IFileSystemRoot fileSystemRoot;

    public ZafiroFile2(ZafiroPath path, IFileSystemRoot fileSystemRoot)
    {
        this.path = path;
        this.fileSystemRoot = fileSystemRoot;
    }

    public IObservable<byte> Contents => fileSystemRoot.Contents(path);
}