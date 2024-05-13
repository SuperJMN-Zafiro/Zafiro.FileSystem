namespace Zafiro.FileSystem.Unix;

public class UnixRoot : UnixDir
{
    public UnixRoot() : base("")
    {
    }
    
    public UnixRoot(IEnumerable<UnixNode> nodes) : base("", nodes)
    {
    }
}

public class RootedUnixFile
{
    public RootedUnixFile(ZafiroPath parent, UnixFile unixFile)
    {
        UnixFile = unixFile;
        Parent = parent;
    }

    public UnixFile UnixFile { get;  }
    public ZafiroPath Parent { get; }
}