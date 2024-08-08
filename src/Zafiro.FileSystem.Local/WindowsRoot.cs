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

    public async Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        return Result.Failure<IMutableDirectory>("Can't create subdirectories here");
    }

    public async Task<Result> DeleteFile(string name)
    {
        return Result.Failure<IMutableDirectory>("Can't delete files here");
    }

    public async Task<Result> DeleteSubdirectory(string name)
    {
        return Result.Failure<IMutableDirectory>("Can't delete subdirectories from root");
    }

    public IObservable<Result<IEnumerable<IMutableNode>>> Children
    {
        get
        {
            var result = Result.Try(() =>
                FileSystem.DriveInfo.GetDrives().Select(driveInfo => driveInfo.RootDirectory)
                    .Select(info => (IMutableNode)new Directory(info)));
            return Observable.Return(result);
        }
    }

    public async Task<Result<IMutableFile>> CreateFile(string entryName)
    {
        return Result.Failure<IMutableFile>("Can't create files in root");
    }
}