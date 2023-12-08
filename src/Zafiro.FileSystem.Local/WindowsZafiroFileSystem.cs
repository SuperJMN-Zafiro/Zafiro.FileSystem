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

    public override async Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path)
    {
        if (path == ZafiroPath.Empty)
        {
            return Result.Success(Enumerable.Empty<ZafiroPath>());
        }
        
        return Result.Try(() => FileSystem.Directory.GetFiles(PathToFileSystem(path)).Select(FileSystemToZafiroPath));
    }

    public override async Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path)
    {
        return Result.Try(GetDirs);

        IEnumerable<ZafiroPath> GetDirs()
        {
            if (path == ZafiroPath.Empty)
            {
                var zafiroPaths = FileSystem.DriveInfo.GetDrives().Select(i => i.RootDirectory.FullName[..^1]).Select(x => x.ToZafiroPath());
                return zafiroPaths;
            }

            var paths = FileSystem.Directory.GetDirectories(PathToFileSystem(path)).Select(FileSystemToZafiroPath);
            return paths;
        }
    }

    public override async Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path)
    {
        return Result.Try(() =>
        {
            var info = FileSystem.FileInfo.New(PathToFileSystem(path));
            var isHidden = path.RouteFragments.Count() != 1 && info.Attributes.HasFlag(FileAttributes.Hidden);
            return new DirectoryProperties(isHidden, info.CreationTime);
        });
    }
}