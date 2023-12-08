namespace Zafiro.FileSystem.Local;

public class LinuxZafiroFileSystem : ZafiroFileSystemBase
{
    public LinuxZafiroFileSystem(System.IO.Abstractions.IFileSystem fileSystem) : base(fileSystem)
    {
    }

    public override string PathToFileSystem(ZafiroPath path) => "/" + path;
    public override ZafiroPath FileSystemToZafiroPath(string fileSystemPath) => fileSystemPath[1..];

    public override async Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = default) => Result.Try(() => FileSystem.Directory.GetFiles(PathToFileSystem(path)).Select(s => (ZafiroPath) s));
    public override async Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default)
    {
        return Result.Try(() => FileSystem.Directory.GetDirectories(path).Select(x => x.ToZafiroPath()));
    }

    public override async Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path)
    {
        return Result.Try(() =>
        {
            var info = FileSystem.FileInfo.New(PathToFileSystem(path));
            return new DirectoryProperties(info.Attributes.HasFlag(FileAttributes.Hidden), info.CreationTime);
        });
    }
}