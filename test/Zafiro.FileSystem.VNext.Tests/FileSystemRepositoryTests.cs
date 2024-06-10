using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using Zafiro.FileSystem.Local;
using Zafiro.Reactive;

namespace Zafiro.FileSystem.VNext.Tests;

public class FileSystemRepositoryTests
{
    [Fact]
    public async Task DDTest()
    {
        var fs = new LocalFileSystem(new System.IO.Abstractions.FileSystem());
        var folder = fs.GetFolder("home/jmn/Escritorio");


        folder.Files.OnItemAdded(file => { }).Subscribe();
        
        var result = await folder.AddOrUpdate(new[] { new File("Pepito.txt", "Hehehe") });
        await Task.Delay(10000);
        result.Should().Succeed();
    }
}

public class LocalFileSystem(IFileSystem fileSystem)
{
    public IFileSystem FileSystem { get; } = fileSystem;

    public ObservableFolder GetFolder(ZafiroPath path)
    {
        return new ObservableFolder(FileSystem.DirectoryInfo.New("/" + path));
    }
}

public class ObservableFolder
{
    private readonly SourceCache<IFile,string> filesCache;
    private readonly SourceCache<ObservableFolder,string> dirsCache;
    public IDirectoryInfo DirectoryInfo { get; }

    public ObservableFolder(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
        filesCache =  new SourceCache<IFile, string>(x => x.Name);
        //dirsCache =  new SourceCache<ObservableFolder, string>(x => x.Name);
        
        filesCache.AddOrUpdate(directoryInfo.GetFiles().Select(info => new DotNetFile(info)));
        //dirsCache.AddOrUpdate(directoryInfo.GetDirectories().Select(info => new ObservableFolder(info)));

        Files = Observable
            .Timer(TimeSpan.FromSeconds(2))
            .Select(_ => Observable.Start(() => DirectoryInfo.GetFiles().Select(d => (IFile)new DotNetFile(d)), TaskPoolScheduler.Default))
            .Repeat()
            .Concat().EditDiff(x => x.Name);
        
        //Directories = dirsCache.Connect();
        //AllDirs = Directories.MergeManyChangeSets(folder => folder.Directories);
        //AllFiles = AllDirs.MergeManyChangeSets(x => x.Files);
    }
    
    public IObservable<IChangeSet<IFile,string>> AllFiles { get; }

    public IObservable<IChangeSet<ObservableFolder,string>> AllDirs { get; }

    public IObservable<IChangeSet<ObservableFolder,string>> Directories { get; }

    public IObservable<IChangeSet<IFile, string>> Files { get; }

    public string Name => DirectoryInfo.Name;

    public Task<Result> AddOrUpdate(File[] files)
    {
        return files.Select(AddOrUpdate).Combine();
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