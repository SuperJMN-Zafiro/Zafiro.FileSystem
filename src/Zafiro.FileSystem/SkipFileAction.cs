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

    public IObservable<IProportionProgress> Progress => Observable.Return(new ProportionProgress());

    public Task<Result> Sync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success());
    }
}