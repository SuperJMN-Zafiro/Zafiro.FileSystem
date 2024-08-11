using System.Reactive.Linq;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.Local;

public class Directory : IMutableDirectory, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    public IDirectoryInfo DirectoryInfo { get; }
    private readonly SourceCache<IMutableNode, string> childrenCache = new(x => x.GetKey());

    public Directory(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
    }

    private IObservable<long> Updater()
    {
        return Observable
            .Timer(TimeSpan.FromSeconds(4))
            .Do(l => TryUpdate())
            .Repeat()
            .StartWith();
    }

    private void TryUpdate()
    {
        GetNodes().Tap(nodes => childrenCache.EditDiff(nodes, (a, b) => a.GetKey() == b.GetKey()));
    }

    public string Name => DirectoryInfo.Name.Replace("\\", "");

    public async Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        return Result.Try(() => DirectoryInfo.CreateSubdirectory(name))
            .Map(directoryInfo => (IMutableDirectory)new Directory(directoryInfo))
            .Tap(d => childrenCache.AddOrUpdate(d));
    }

    public async Task<Result> DeleteFile(string name)
    {
        var filePath = DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, name);
        var file = DirectoryInfo.FileSystem.FileInfo.New(filePath);
        
        return Result.Try(() =>
        {
            file.Delete();
        })
        .Tap(() => childrenCache.Remove(name));
    }

    public async Task<Result> DeleteSubdirectory(string name)
    {
        var dirPath = DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, name);
        var dir = DirectoryInfo.FileSystem.DirectoryInfo.New(dirPath);
        
        return Result.Try(() =>
        {
            dir.Delete();
        })
        .Tap(() => childrenCache.Remove(name + "/"));
    }

    public IObservable<IChangeSet<IMutableNode, string>> Children
    {
        get
        {
            var observable = childrenCache
                .Connect()
                .DisposeMany();
            
            TryUpdate();
            Updater().Subscribe().DisposeWith(disposables);
            return observable;
        }
    }

    private Result<IEnumerable<IMutableNode>> GetNodes()
    {
        return Result.Try(() =>
        {
            var files = DirectoryInfo.GetFiles().Select(info => (IMutableNode)new File(info));
            var dirs = DirectoryInfo.GetDirectories().Select(x => (IMutableNode)new Directory(x));
            var nodes = files.Concat(dirs);
            return nodes;
        });
    }

    public Task<Result<IMutableFile>> CreateFile(string entryName)
    {
        var fs = DirectoryInfo.FileSystem;
        var file = new File(fs.FileInfo.New(fs.Path.Combine(DirectoryInfo.FullName, entryName)));
        return Task.FromResult<Result<IMutableFile>>(file).Tap(f => childrenCache.AddOrUpdate(f));
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

    public override string? ToString()
    {
        return DirectoryInfo.ToString();
    }

    public void Dispose()
    {
        disposables.Dispose();
        childrenCache.Dispose();
    }
}