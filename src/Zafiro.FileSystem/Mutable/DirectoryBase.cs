using System.Reactive.Disposables;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using DynamicData;

namespace Zafiro.FileSystem.Mutable;

public abstract class DirectoryBase : IMutableDirectory, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly SourceCache<IMutableNode, string> childrenCache = new(x => x.GetKey());

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
        GetChildren().Tap(nodes => childrenCache.EditDiff(nodes, (a, b) => a.GetKey() == b.GetKey()));
    }

    public abstract string Name { get; }

    public Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        return CreateSubdirectoryCore(name)
            .Tap(d => childrenCache.AddOrUpdate(d));
    }

    protected abstract Task<Result<IMutableDirectory>> CreateSubdirectoryCore(string name);

    public Task<Result> DeleteFile(string name)
    {
        return DeleteFileCore(name).Tap(() => childrenCache.Remove(name));
    }

    protected abstract Task<Result> DeleteFileCore(string name);

    public Task<Result> DeleteSubdirectory(string name)
    {
        return DeleteSubdirectoryCore(name)
            .Tap(() => childrenCache.Remove(name + "/"));
    }

    protected abstract Task<Result> DeleteSubdirectoryCore(string name);

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

    protected abstract Result<IEnumerable<IMutableNode>> GetChildren();

    public Task<Result<IMutableFile>> CreateFile(string entryName)
    {
        return CreateFileCore(entryName).Tap(f => childrenCache.AddOrUpdate(f));
    }

    protected abstract Task<Result<IMutableFile>> CreateFileCore(string entryName);

    public abstract bool IsHidden { get; }

    public void Dispose()
    {
        disposables.Dispose();
        childrenCache.Dispose();
    }
}