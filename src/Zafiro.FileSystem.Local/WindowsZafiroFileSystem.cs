using System.IO.Abstractions;

namespace Zafiro.FileSystem.Local;

public class WindowsZafiroFileSystem : ZafiroFileSystemBase
{
    public WindowsZafiroFileSystem(IFileSystem fileSystem) : base(fileSystem)
    {
    }

    public override string PathToFileSystem(ZafiroPath path)
    {
        var s = path.ToString();

        if (s.EndsWith(":"))
        {
            s += "\\";
        }

        return s.Replace("/", "\\");
    }

    public override ZafiroPath FileSystemToZafiroPath(string fileSystemPath) => fileSystemPath.Replace("\\", "/");

    public override async Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default)
    {
        if (path == ZafiroPath.Empty)
        {
            return Result.Try(() => FileSystem.DriveInfo.GetDrives().Select(i => i.RootDirectory.FullName[..^1]).Select(x => x.ToZafiroPath()));
        }

        return await base.GetDirectoryPaths(path, ct).ConfigureAwait(false);
    }

    public override async Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path)
    {
        return Result.Try(() =>
        {
            var info = FileSystem.FileInfo.New(PathToFileSystem(path));
            var isHidden = path.Name().StartsWith(".") || path.RouteFragments.Count() != 1 && info.Attributes.HasFlag(FileAttributes.Hidden);
            return new DirectoryProperties(isHidden, info.CreationTime);
        });
    }
}