using System.IO.Abstractions;
using System.Reactive.Disposables;
using DynamicData;
using Zafiro.FileSystem.DynamicData;

namespace Zafiro.FileSystem.Local.Mutable;

public class LocalDynamicDirectory : IDynamicDirectory
{
    private readonly CompositeDisposable disposable = new();
    private readonly LocalFileList localFileList;
    private readonly LocalDirectoryList localDirectoryList;
    public IDirectoryInfo DirectoryInfo { get; }

    public LocalDynamicDirectory(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;

        localFileList = new LocalFileList(directoryInfo).DisposeWith(disposable);
        Files = localFileList.Connect();
        
        localDirectoryList = new LocalDirectoryList(directoryInfo).DisposeWith(disposable);
        Directories = localDirectoryList.Connect();
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
}

