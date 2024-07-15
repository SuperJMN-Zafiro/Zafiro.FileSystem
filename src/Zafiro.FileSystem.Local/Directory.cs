using System.Reactive.Linq;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.Local;

public class Directory : IMutableDirectory
{
    public IDirectoryInfo DirectoryInfo { get; }

    public Directory(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
    }

    public string Name => DirectoryInfo.Name.Replace("\\", "");

    public async Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        return Result.Try(() => DirectoryInfo.CreateSubdirectory(name))
            .Map(directoryInfo => (IMutableDirectory)new Directory(directoryInfo));
    }

    public async Task<Result> Delete()
    {
        return Result.Try(() => DirectoryInfo.Delete());
    }

    public IObservable<Result<IEnumerable<IMutableNode>>> Children
    {
        get
        {
            var childrenProp = Result.Try(() =>
            {
                var files = DirectoryInfo.GetFiles().Select(info => (IMutableNode)new File(info));
                var dirs = DirectoryInfo.GetDirectories().Select(x => (IMutableNode)new Directory(x));
                var nodes = files.Concat(dirs);
                return nodes;
            });
            
            return Observable.Return(childrenProp);
        }
    }

    public Task<Result<IMutableFile>> GetFile(string entryName)
    {
        return Task.FromResult<Result<IMutableFile>>(new File(DirectoryInfo.FileSystem.FileInfo.New(entryName)));
    }

    public bool IsHidden
    {
        get
        {
            if (DirectoryInfo.Parent == null)
            {
                return false;
            }
            
            return (DirectoryInfo.Attributes & FileAttributes.Hidden) != 0;
        }
    }

    public Task<Result<bool>> Exists()
    {
        throw new NotImplementedException();
    }

    public async Task<Result> Create()
    {
        return Result.Try(() => DirectoryInfo.Create());
    }

    public override string? ToString()
    {
        return DirectoryInfo.ToString();
    }
}