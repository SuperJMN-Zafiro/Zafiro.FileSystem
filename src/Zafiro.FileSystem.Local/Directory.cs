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

    public async Task<Result> DeleteFile(string name)
    {
        var filePath = DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, name);
        var file = DirectoryInfo.FileSystem.FileInfo.New(filePath);
        
        return Result.Try(() =>
        {
            file.Delete();
        });
    }

    public async Task<Result> DeleteSubdirectory(string name)
    {
        var dirPath = DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, name);
        var dir = DirectoryInfo.FileSystem.DirectoryInfo.New(dirPath);
        
        return Result.Try(() =>
        {
            dir.Delete();
        });
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

    public Task<Result<IMutableFile>> CreateFile(string entryName)
    {
        var fs = DirectoryInfo.FileSystem;
        var file = new File(fs.FileInfo.New(fs.Path.Combine(DirectoryInfo.FullName, entryName)));
        return Task.FromResult<Result<IMutableFile>>(file);
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