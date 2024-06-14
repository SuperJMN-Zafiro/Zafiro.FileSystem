using System.IO.Abstractions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;

namespace Zafiro.FileSystem.Local.Mutable;

public class LocalFileList : IDisposable
{
    public IDirectoryInfo DirectoryInfo { get; }
    private readonly SourceCache<IFile,string> filesCache;
    private readonly CompositeDisposable disposable = new();

    public LocalFileList(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
        filesCache = new SourceCache<IFile, string>(x => x.Name);
        Update().Tap(files => filesCache.AddOrUpdate(files, new LambdaComparer<IFile>((a, b) => Equals(a.Name, b.Name))));
        TimeBasedUpdater()
            .DisposeWith(disposable);
    }

    public IScheduler Scheduler { get; set; } = System.Reactive.Concurrency.Scheduler.Default;

    public IObservable<IChangeSet<IFile, string>> Connect()
    {
        return filesCache.Connect().DisposeMany();
    }
    
    public Task<Result> AddOrUpdate(params IFile[] files)
    {
        return files.Select(AddOrUpdate).Combine();
    }
    
    public async Task<Result> Delete(string name)
    {
        var result = Result.Try(() => DirectoryInfo.FileSystem.File.Delete(DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, name)));
        result.Tap(() => filesCache.Remove(name));
        return result;
    }

    private async Task<Result> AddOrUpdate(IFile file)
    {
        var path = DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, file.Name);
        await using var fileSystemStream = DirectoryInfo.FileSystem.File.Create(path);
        var result = await file.DumpTo(fileSystemStream);
        result.Tap(() => filesCache.AddOrUpdate(file));
        return result;
    }
    
    private IDisposable TimeBasedUpdater()
    {
        return Observable.Timer(TimeSpan.FromSeconds(2), scheduler: Scheduler)
            .Repeat()
            .Do(_ =>
            {
                Update().Tap(files => filesCache.EditDiff(files, (a, b) => Equals(a.Name, b.Name)));
            })
            .Subscribe();
    }

    private Result<IEnumerable<IFile>> Update()
    { 
        return Result.Try(() => DirectoryInfo.GetFiles().Select(d => (IFile)new DotNetFile(d)));
    }

    public void Dispose()
    {
        filesCache.Dispose();
        disposable.Dispose();
    }
}