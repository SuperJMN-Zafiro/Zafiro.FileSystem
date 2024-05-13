namespace Zafiro.FileSystem.Unix;

public class RootedUnixFile : RootedFile
{
    public RootedUnixFile(ZafiroPath parent, UnixFile unixFile) : base(parent, unixFile)
    {
        UnixFile = unixFile;
        Parent = parent;
    }

    public UnixFile UnixFile { get;  }
    public ZafiroPath Parent { get; }
}