using System.IO.Abstractions;
using System.Reactive.Disposables;
using CSharpFunctionalExtensions;
using DynamicData;

namespace Zafiro.FileSystem.DynamicData;

public class DynamicDirectory
{
    private readonly CompositeDisposable disposable = new();
    private readonly FileList fileList;
    private readonly DirectoryList directoryList;
    public IDirectoryInfo DirectoryInfo { get; }

    public DynamicDirectory(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;

        fileList = new FileList(directoryInfo).DisposeWith(disposable);
        Files = fileList.Connect();
        
        directoryList = new DirectoryList(directoryInfo).DisposeWith(disposable);
        Directories = directoryList.Connect();
    }
    
    public IObservable<IChangeSet<DynamicDirectory,string>> Directories { get; }

    public IObservable<IChangeSet<IFile, string>> Files { get; }

    public string Name => DirectoryInfo.Name;

    public Task<Result> DeleteFile(string name)
    {
        return fileList.Delete(name);
    }

    public Task<Result> AddOrUpdateFile(params IFile[]  files)
    {
        return fileList.AddOrUpdate(files);
    }
}

