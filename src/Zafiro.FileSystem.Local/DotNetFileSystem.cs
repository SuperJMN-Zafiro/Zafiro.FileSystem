using Zafiro.FileSystem.Mutable;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Zafiro.FileSystem.Local;

public class DotNetFileSystem : Mutable.IFileSystem
{
    public IFileSystem FileSystem { get; }

    public DotNetFileSystem(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public Task<Result<IRooted<IMutableDirectory>>> Get(ZafiroPath path)
    {
        if (OperatingSystem.IsLinux())
        {
            var translatedPath =  "/" + path;

            return Task.FromResult(Result
                .Try(() => FileSystem.DirectoryInfo.New(translatedPath))
                .Map(d => new DotNetMutableDirectory(new DotNetDirectory(d)))
                .Map(directory => (IRooted<IMutableDirectory>)new Rooted<IMutableDirectory>(path, directory)));
        }

        if (OperatingSystem.IsWindows())
        {
            if (path == ZafiroPath.Empty)
            {
                var mutableDirectory = (IMutableDirectory)new WindowsRoot(FileSystem);
                Result<IMutableDirectory> success = Result.Success(mutableDirectory);
                IRooted<IMutableDirectory> pp = new Rooted<IMutableDirectory>(ZafiroPath.Empty, mutableDirectory);
                return Task.FromResult<Result<IRooted<IMutableDirectory>>>(Result.Success(pp));
            }
            
            return Task.FromResult(Result
                .Try(() => FileSystem.DirectoryInfo.New(path))
                .Map(d => new DotNetMutableDirectory(new DotNetDirectory(d)))
                .Map(directory => (IRooted<IMutableDirectory>)new Rooted<IMutableDirectory>(path, directory)));
        }
        
        throw new NotSupportedException("Only supported OSes are Windows and Linux for now");
    }

    public ZafiroPath InitialPath
    {
        get
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return OperatingSystem.IsWindows() ? FromWindows(folderPath) : folderPath;
        }
    }

    private ZafiroPath FromWindows(string folderPath)
    {
        return folderPath.Replace("\\", "/");
    }
}