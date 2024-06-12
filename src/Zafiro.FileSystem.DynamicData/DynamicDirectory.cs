using System.IO.Abstractions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using DynamicData;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.DynamicData;

public class DynamicDirectory
{
    private readonly SourceCache<IFile,string> filesCache;
    private readonly SourceCache<DynamicDirectory,string> dirsCache;
    private readonly CompositeDisposable disposable = new();
    public IDirectoryInfo DirectoryInfo { get; }

    public DynamicDirectory(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
        filesCache =  new SourceCache<IFile, string>(x => x.Name);
        //dirsCache =  new SourceCache<ObservableFolder, string>(x => x.Name);
        
        filesCache.AddOrUpdate(Update());
        //dirsCache.AddOrUpdate(directoryInfo.GetDirectories().Select(info => new ObservableFolder(info)));

        TimeBasedUpdater()
            .DisposeWith(disposable);

        Files = filesCache.Connect();
        
        //Directories = dirsCache.Connect();
        //AllDirs = Directories.MergeManyChangeSets(folder => folder.Directories);
        //AllFiles = AllDirs.MergeManyChangeSets(x => x.Files);
    }

    private IDisposable TimeBasedUpdater()
    {
        return Observable.Timer(TimeSpan.FromSeconds(10), scheduler: Scheduler)
            .Do(_ =>
            {
                filesCache.Edit(updater => updater.Load(Update()));
            })
            .Subscribe();
    }

    public IScheduler Scheduler { get; set; } = System.Reactive.Concurrency.Scheduler.Default;

    private IEnumerable<IFile> Update()
    {
        return DirectoryInfo.GetFiles().Select(d => (IFile)new DotNetFile(d));
    }

    public IObservable<IChangeSet<IFile,string>> AllFiles { get; }

    public IObservable<IChangeSet<DynamicDirectory,string>> AllDirs { get; }

    public IObservable<IChangeSet<DynamicDirectory,string>> Directories { get; }

    public IObservable<IChangeSet<IFile, string>> Files { get; }

    public string Name => DirectoryInfo.Name;

    public Task<Result> AddOrUpdate(params File[] files)
    {
        return files.Select(AddOrUpdate).Combine();
    }

    public async Task<Result> Delete(string name)
    {
        var result = Result.Try(() => DirectoryInfo.FileSystem.File.Delete(DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, name)));
        result.Tap(() => filesCache.Remove(name));
        return result;
    }

    private async Task<Result> AddOrUpdate(File file)
    {
        var path = DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, file.Name);
        await using var fileSystemStream = DirectoryInfo.FileSystem.File.Create(path);
        var result = await file.DumpTo(fileSystemStream);
        result.Tap(() => filesCache.AddOrUpdate(file));
        return result;
    }
}

