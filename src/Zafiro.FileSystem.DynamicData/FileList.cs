using System.IO.Abstractions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using DynamicData;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.DynamicData;

public class FileList : IDisposable
{
    public IDirectoryInfo DirectoryInfo { get; }
    private readonly IDirectoryInfo dynamicDirectory;
    private readonly SourceCache<IFile,string> filesCache;
    private readonly CompositeDisposable disposable = new();

    public FileList(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
        filesCache = new SourceCache<IFile, string>(x => x.Name);
        filesCache.AddOrUpdate(Update(), new LambdaComparer<IFile>((a, b) => Equals(a.Name, b.Name)));
        TimeBasedUpdater()
            .DisposeWith(disposable);
    }

    public IScheduler Scheduler { get; set; } = System.Reactive.Concurrency.Scheduler.Default;

    public IObservable<IChangeSet<IFile, string>> Connect()
    {
        return filesCache.Connect();
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
        return Observable.Timer(TimeSpan.FromSeconds(10), scheduler: Scheduler)
            .Repeat()
            .Do(_ =>
            {
                filesCache.EditDiff(Update(), (a, b) => Equals(a.Name, b.Name));
            })
            .Subscribe();
    }

    private IEnumerable<IFile> Update()
    {
        return DirectoryInfo.GetFiles().Select(d => (IFile)new DotNetFile(d));
    }

    public void Dispose()
    {
        filesCache.Dispose();
        disposable.Dispose();
    }
}