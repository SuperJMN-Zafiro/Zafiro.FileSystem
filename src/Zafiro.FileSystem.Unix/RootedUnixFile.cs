namespace Zafiro.FileSystem.Unix;

public class RootedUnixFile : RootedFile
{
    public RootedUnixFile(ZafiroPath path, UnixFile unixFile) : base(path, unixFile)
    {
        UnixFile = unixFile;
    }

    public UnixFile UnixFile { get;  }
}