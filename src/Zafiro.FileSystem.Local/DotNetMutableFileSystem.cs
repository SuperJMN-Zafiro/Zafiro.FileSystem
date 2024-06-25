using Zafiro.FileSystem.Mutable;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Zafiro.FileSystem.Local;

public class DotNetMutableFileSystem : Mutable.IMutableFileSystem
{
    public IFileSystem FileSystem { get; }

    public DotNetMutableFileSystem(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public Task<Result<IRooted<IMutableDirectory>>> GetDirectory(ZafiroPath path)
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
                IRooted<IMutableDirectory> pp = new Rooted<IMutableDirectory>(ZafiroPath.Empty, mutableDirectory);
                return Task.FromResult(Result.Success(pp));
            }
            
            return Task.FromResult(Result
                .Try(() => FileSystem.DirectoryInfo.New(path))
                .Map(d => new DotNetMutableDirectory(new DotNetDirectory(d)))
                .Map(directory => (IRooted<IMutableDirectory>)new Rooted<IMutableDirectory>(path, directory)));
        }
        
        throw new NotSupportedException("Only supported OSes are Windows and Linux for now");
    }

    public Task<Result<IRooted<IMutableFile>>> GetFile(ZafiroPath path)
    {
        if (OperatingSystem.IsLinux())
        {
            var translatedPath =  "/" + path;

            return Task.FromResult(Result
                .Try(() => FileSystem.FileInfo.New(translatedPath))
                .Map(d => new DotNetMutableFile(d))
                .Map(directory => (IRooted<IMutableFile>)new Rooted<IMutableFile>(path, directory)));
        }

        return Task.FromResult(Result.Failure<IRooted<IMutableFile>>("Not implemented"));
    }

    public ZafiroPath InitialPath
    {
        get
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return OperatingSystem.IsWindows() ? FromWindows(folderPath) : folderPath[1..];
        }
    }

    private ZafiroPath FromWindows(string folderPath)
    {
        return folderPath.Replace("\\", "/");
    }
}