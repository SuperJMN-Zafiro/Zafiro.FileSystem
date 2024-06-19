using System.Reactive.Subjects;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Zafiro.FileSystem.Local;

public class WindowsRoot : IMutableDirectory
{
    public IFileSystem FileSystem { get; }

    public WindowsRoot(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public string Name { get; } = "<root>";
    
    public Task<Result<IEnumerable<INode>>> Children()
    {
        return MutableChildren().MapEach(n => (INode)n);
    }

    public bool IsHidden => false;
    
    public async Task<Result<IEnumerable<IMutableNode>>> MutableChildren()
    {
        return Result.Try(() =>
            FileSystem.DriveInfo.GetDrives().Select(driveInfo => driveInfo.RootDirectory)
                .Select(info => (IMutableNode)new DotNetMutableDirectory(new DotNetDirectory(info))));
    }

    public async Task<Result> AddOrUpdate(IFile data, ISubject<double>? progress = null)
    {
        return Result.Failure("Cannot create directory here");
    }

    public async Task<Result<IMutableFile>> CreateFile(string name)
    {
        return Result.Failure<IMutableFile>("Cannot create directory here");
    }

    public async Task<Result<IMutableDirectory>> CreateDirectory(string name)
    {
        return Result.Failure<IMutableDirectory>("Cannot create directory here");
    }

    public async Task<Result> Delete()
    {
        return Result.Failure("Cannot delete anything here");
    }
}