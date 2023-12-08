namespace Zafiro.FileSystem.Local;

public class LinuxZafiroFileSystem : ZafiroFileSystemBase
{
    public LinuxZafiroFileSystem(System.IO.Abstractions.IFileSystem fileSystem) : base(fileSystem)
    {
    }

    public override string PathToFileSystem(ZafiroPath path) => "/" + path;
    public override ZafiroPath FileSystemToZafiroPath(string fileSystemPath) => fileSystemPath[1..];

    protected override IEnumerable<ZafiroPath> GetDirectories(ZafiroPath path)
    {
        return FileSystem.Directory.GetDirectories(path).Select(x => x.ToZafiroPath());
    }
}