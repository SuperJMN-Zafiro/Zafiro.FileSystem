using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Actions;

namespace Zafiro.FileSystem;

public class SkipFileAction : ISyncAction
{
    public SkipFileAction(IZafiroFile source, Maybe<IZafiroFile> destination)
    {
        Source = source;
        Destination = destination;
    }

    public IZafiroFile Source { get; }
    public Maybe<IZafiroFile> Destination { get; }

    public IObservable<IProgress> Progress => Observable.Return(new Progress());

    public Task<Result> Sync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success());
    }
}