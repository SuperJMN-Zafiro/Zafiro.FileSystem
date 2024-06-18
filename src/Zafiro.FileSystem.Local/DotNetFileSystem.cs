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
        var translatedPath = OperatingSystem.IsLinux() ? "/" + path : path.ToString();
        
        return Task.FromResult(Result
            .Try(() => FileSystem.DirectoryInfo.New(translatedPath))
            .Map(d => new DotNetMutableDirectory(new DotNetDirectory(d)))
            .Map(directory => (IRooted<IMutableDirectory>)new Rooted<IMutableDirectory>(path, directory)));
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