using System.IO.Abstractions;
using System.Reactive.Disposables;
using DynamicData;
using Zafiro.FileSystem.DynamicData;

namespace Zafiro.FileSystem.Local.Mutable;

public static class DynamicMixin
{
    public static IObservable<IChangeSet<IFile, string>> AllFiles(this IDynamicDirectory directory) => directory.Files.MergeChangeSets(directory.AllDirectories().MergeManyChangeSets(x => x.AllFiles()));
    public static IObservable<IChangeSet<IDynamicDirectory, string>> AllDirectories(this IDynamicDirectory directory) => directory.Directories.MergeManyChangeSets(x => x.AllDirectories());
}

public class LocalDynamicDirectory : IDynamicDirectory, IDisposable
{
    private readonly CompositeDisposable disposable = new();
    private readonly LocalFileList localFileList;
    private readonly LocalDirectoryList localDirectoryList;
    public IDirectoryInfo DirectoryInfo { get; }

    public LocalDynamicDirectory(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;

        localFileList = new LocalFileList(directoryInfo).DisposeWith(disposable);
        Files = localFileList
            .Connect();
        
        localDirectoryList = new LocalDirectoryList(directoryInfo).DisposeWith(disposable);
        Directories = localDirectoryList
            .Connect()
            .DisposeMany();
    }
    
    public IObservable<IChangeSet<IDynamicDirectory,string>> Directories { get; }

    public IObservable<IChangeSet<IFile, string>> Files { get; }

    public string Name => DirectoryInfo.Name;
    
    public Task<Result> DeleteFile(string name)
    {
        return localFileList.Delete(name);
    }

    public Task<Result> AddOrUpdateFile(params IFile[]  files)
    {
        return localFileList.AddOrUpdate(files);
    }

    public void Dispose()
    {
        disposable.Dispose();
        localFileList.Dispose();
        localDirectoryList.Dispose();
    }
}

