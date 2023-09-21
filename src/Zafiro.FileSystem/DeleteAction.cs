using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Actions;

namespace Zafiro.FileSystem;

public class DeleteAction : ISyncAction
{
    private readonly IZafiroFile file;

    public DeleteAction(IZafiroFile file)
    {
        this.file = file;
    }

    public IZafiroFile Source => file;
    public IObservable<IProgress> Progress => Observable.Return(new Progress());
    public Task<Result> Sync(CancellationToken cancellationToken)
    {
        return file.Delete();
    }
}