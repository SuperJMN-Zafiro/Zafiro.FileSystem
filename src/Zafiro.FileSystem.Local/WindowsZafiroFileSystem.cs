using System.IO.Abstractions;

namespace Zafiro.FileSystem.Local;

public class WindowsZafiroFileSystem : ZafiroFileSystemBase
{
    public WindowsZafiroFileSystem(IFileSystem fileSystem) : base(fileSystem)
    {
    }

    public override string PathToFileSystem(ZafiroPath path) => path.ToString().Replace("/", "\\");

    public override ZafiroPath FileSystemToZafiroPath(string fileSystemPath) => fileSystemPath.Replace("\\", "/");

    protected override IEnumerable<ZafiroPath> GetDirectories(ZafiroPath path)
    {
        if (path == ZafiroPath.Empty)
        {
            return FileSystem.DriveInfo.GetDrives().Select(i => i.RootDirectory.FullName[..^1]).Select(x => x.ToZafiroPath());
        }

        return FileSystem.Directory.GetDirectories(path).Select(x => x.ToZafiroPath());
    }
}