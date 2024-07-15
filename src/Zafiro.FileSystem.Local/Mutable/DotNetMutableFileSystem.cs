using Zafiro.FileSystem.Mutable;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Zafiro.FileSystem.Local.Mutable;

public class DotNetMutableFileSystem : IMutableFileSystem
{
    public IFileSystem FileSystem { get; }

    public DotNetMutableFileSystem(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public Task<Result<IMutableDirectory>> GetDirectory(ZafiroPath path)
    {
        if (OperatingSystem.IsLinux())
        {
            var translatedPath =  "/" + path;

            return Task.FromResult(Result
                .Try(() => FileSystem.DirectoryInfo.New(translatedPath))
                .Map(d => (IMutableDirectory)new DotNetMutableDirectory(d)));
        }

        if (OperatingSystem.IsWindows())
        {
            if (path == ZafiroPath.Empty)
            {
                var mutableDirectory = (IMutableDirectory)new WindowsRoot(FileSystem);
                return Task.FromResult(Result.Success(mutableDirectory));
            }
            
            return Task.FromResult(Result
                .Try(() => FileSystem.DirectoryInfo.New(path))
                .Map(d => (IMutableDirectory)new DotNetMutableDirectory(d)));
        }
        
        throw new NotSupportedException("Only supported OSes are Windows and Linux for now");
    }

    public Task<Result<IMutableFile>> GetFile(ZafiroPath path)
    {
        if (OperatingSystem.IsLinux())
        {
            var translatedPath =  "/" + path;

            return Task.FromResult(Result
                .Try(() => FileSystem.FileInfo.New(translatedPath))
                .Map(d =>  (IMutableFile)new DotNetMutableFile(d)));
        }

        if (OperatingSystem.IsWindows())
        {
            return Task.FromResult(Result
                .Try(() => FileSystem.FileInfo.New(path))
                .Map(d => (IMutableFile)new DotNetMutableFile(d)));

        }

        return Task.FromResult(Result.Failure<IMutableFile>("Not implemented"));
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