using System.Reactive.Subjects;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Local.Mutable;
using Zafiro.FileSystem.Mutable;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Zafiro.FileSystem.Local;

public class WindowsRoot : IMutableDirectory
{
    public WindowsRoot(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public IFileSystem FileSystem { get; }

    public string Name { get; } = "<root>";

    public Task<Result<IEnumerable<INode>>> Children()
    {
        Func<IMutableNode, INode> selector = n => n;
        return MutableChildren().ManyMap(selector);
    }

    public bool IsHidden => false;

    public async Task<Result> Create()
    {
        return Result.Failure("Cannot create the root");
    }

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

    public async Task<Result<IMutableFile>> Get(string name)
    {
        return Result.Failure<IMutableFile>("Cannot create directory here");
    }

    public async Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        return Result.Failure<IMutableDirectory>("Cannot create directory here");
    }

    public async Task<Result> Delete()
    {
        return Result.Failure("Cannot delete anything here");
    }
}