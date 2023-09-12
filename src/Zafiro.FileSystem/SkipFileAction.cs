using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.ProgressReporting;

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

    public IObservable<RelativeProgress<long>> Progress => Observable.Return(new RelativeProgress<long>(1, 1));

    public Task<Result> Sync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success());
    }
}