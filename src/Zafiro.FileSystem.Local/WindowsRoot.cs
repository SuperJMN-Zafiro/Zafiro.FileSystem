using System.Reactive.Linq;
using Zafiro.CSharpFunctionalExtensions;
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

    public bool IsHidden => false;

    public async Task<Result<bool>> Exists()
    {
        return true;
    }

    public async Task<Result> Create()
    {
        return Result.Failure("Cannot create the root");
    }

    public async Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        return Result.Failure<IMutableDirectory>("Cannot create directory here");
    }

    public async Task<Result> Delete()
    {
        return Result.Failure("Cannot delete anything here");
    }

    public IObservable<Result<IEnumerable<IMutableNode>>> ChildrenProp
    {
        get
        {
            var result = Result.Try(() =>
                FileSystem.DriveInfo.GetDrives().Select(driveInfo => driveInfo.RootDirectory)
                    .Select(info => (IMutableNode)new Directory(info)));
            return Observable.Return(result);
        }
    }
}