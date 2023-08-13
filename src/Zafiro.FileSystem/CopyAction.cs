using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.ProgressReporting;

namespace Zafiro.FileSystem;

public class CopyAction : ISyncAction
{
    private readonly ISubject<RelativeProgress<long>> progressSubject = new BehaviorSubject<RelativeProgress<long>>(new RelativeProgress<long>(1, 0));

    public CopyAction(IZafiroFile source, IZafiroFile destination)
    {
        Source = source;
        Destination = destination;
    }

    public IZafiroFile Source { get; }

    public IZafiroFile Destination { get; }

    public IObservable<RelativeProgress<long>> Progress => progressSubject.AsObservable();

    public Task<Result> Sync(CancellationToken cancellationToken)
    {
        return Source.Copy(Destination, Maybe<IObserver<RelativeProgress<long>>>.From(progressSubject), cancellationToken: cancellationToken);
    }
}