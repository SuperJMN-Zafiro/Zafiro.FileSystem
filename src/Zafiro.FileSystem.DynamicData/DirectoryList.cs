using System.IO.Abstractions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using DynamicData;

namespace Zafiro.FileSystem.DynamicData;

public class DirectoryList : IDisposable
{
    public IDirectoryInfo DirectoryInfo { get; }
    private readonly SourceCache<DynamicDirectory,string> dirsCache;
    private readonly CompositeDisposable disposable = new();

    public DirectoryList(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
        dirsCache = new SourceCache<DynamicDirectory, string>(x => x.Name);
        dirsCache.AddOrUpdate(Update(), new LambdaComparer<DynamicDirectory>((a, b) => Equals(a.Name, b.Name)));
        TimeBasedUpdater()
            .DisposeWith(disposable);
    }

    public IScheduler Scheduler { get; set; } = System.Reactive.Concurrency.Scheduler.Default;

    public IObservable<IChangeSet<DynamicDirectory, string>> Connect()
    {
        return dirsCache.Connect();
    }

    public async Task<Result> Delete(string name)
    {
        var result = Result.Try(() => DirectoryInfo.FileSystem.Directory.Delete(DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, name)));
        result.Tap(() => dirsCache.Remove(name));
        return result;
    }

    public async Task<Result> AddOrUpdate(string name)
    {
        var path = DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, name);
        var result = Result.Try(() => DirectoryInfo.FileSystem.Directory.CreateDirectory(path));
        result.Tap(d => dirsCache.AddOrUpdate(new DynamicDirectory(d)));
        return result;
    }
    
    private IDisposable TimeBasedUpdater()
    {
        return Observable.Timer(TimeSpan.FromSeconds(10), scheduler: Scheduler)
            .Repeat()
            .Do(_ =>
            {
                dirsCache.EditDiff(Update(), (a, b) => Equals(a.Name, b.Name));
            })
            .Subscribe();
    }

    private IEnumerable<DynamicDirectory> Update()
    {
        return DirectoryInfo.GetDirectories().Select(d => (DynamicDirectory)new DynamicDirectory(d));
    }

    public void Dispose()
    {
        dirsCache.Dispose();
        disposable.Dispose();
    }
}