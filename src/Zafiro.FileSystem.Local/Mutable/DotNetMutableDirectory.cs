using System.Reactive.Linq;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.Local.Mutable;

public class DotNetMutableDirectory : IMutableDirectory
{
    public DotNetDirectory Directory { get; }
    public IDirectoryInfo DirectoryInfo { get; }

    public DotNetMutableDirectory(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
    }

    public string Name => Directory.Name.Replace("\\", "");

    public Task<Result<IEnumerable<INode>>> Children()
    {
        return Directory.Children();
    }

    public async Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        return Result.Try(() => Directory.DirectoryInfo.CreateSubdirectory(name))
            .Map(directoryInfo => (IMutableDirectory)new DotNetMutableDirectory(directoryInfo));
    }

    public async Task<Result> Delete()
    {
        return Result.Try(() => Directory.DirectoryInfo.Delete());
    }

    public IObservable<Result<IEnumerable<IMutableNode>>> ChildrenProp
    {
        get
        {
            var childrenProp = Result.Try(() =>
            {
                var files = DirectoryInfo.GetFiles().Select(info => (IMutableNode)new DotNetMutableFile(info));
                var dirs = DirectoryInfo.GetDirectories().Select(x => (IMutableNode)new DotNetMutableDirectory(x));
                var nodes = files.Concat(dirs);
                return nodes;
            });
            
            return Observable.Return(childrenProp);
        }
    }

    public bool IsHidden
    {
        get
        {
            if (Directory.DirectoryInfo.Parent == null)
            {
                return false;
            }
            
            return (Directory.DirectoryInfo.Attributes & FileAttributes.Hidden) != 0;
        }
    }

    public Task<Result<bool>> Exists()
    {
        throw new NotImplementedException();
    }

    public async Task<Result> Create()
    {
        return Result.Try(() => Directory.DirectoryInfo.Create());
    }

    public override string? ToString()
    {
        return Directory.ToString();
    }
}