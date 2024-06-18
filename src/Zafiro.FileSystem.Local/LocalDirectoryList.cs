using System.IO.Abstractions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Zafiro.FileSystem.DynamicData;

namespace Zafiro.FileSystem.Local;

public class LocalDirectoryList : IDisposable
{
    public IDirectoryInfo DirectoryInfo { get; }
    private readonly SourceCache<IDynamicDirectory, string> dirsCache;
    private readonly CompositeDisposable disposable = new();

    public LocalDirectoryList(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
        dirsCache = new SourceCache<IDynamicDirectory, string>(x => x.Name);
        Update().Tap(files => dirsCache.AddOrUpdate(files, new LambdaComparer<INamed>((a, b) => Equals(a.Name, b.Name))));
        TimeBasedUpdater()
            .DisposeWith(disposable);
    }

    public IScheduler Scheduler { get; set; } = System.Reactive.Concurrency.Scheduler.Default;

    public IObservable<IChangeSet<IDynamicDirectory, string>> Connect()
    {
        return dirsCache.Connect().DisposeMany();
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
        result.Tap(d => dirsCache.AddOrUpdate(new LocalDynamicDirectory(d)));
        return result;
    }

    private IDisposable TimeBasedUpdater()
    {
        return Observable.Timer(TimeSpan.FromSeconds(2), scheduler: Scheduler)
            .Repeat()
            .Do(_ =>
            {
                Update().Tap(files => dirsCache.EditDiff(files, (a, b) => Equals(a.Name, b.Name)));
            })
            .Subscribe();
    }

    private Result<IEnumerable<IDynamicDirectory>> Update()
    {
        return Result.Try(() => DirectoryInfo.GetDirectories().Select(d => (IDynamicDirectory)new LocalDynamicDirectory(d)));
    }

    public void Dispose()
    {
        dirsCache.Dispose();
        disposable.Dispose();
    }
}