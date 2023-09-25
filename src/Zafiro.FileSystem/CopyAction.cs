using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Actions;

namespace Zafiro.FileSystem;

public class CopyAction : ISyncAction
{
    private readonly BehaviorSubject<IProgress> progressSubject = new(new Progress());

    public CopyAction(IZafiroFile source, IZafiroFile destination)
    {
        Source = source;
        Destination = destination;
    }

    public IZafiroFile Source { get; }

    public IZafiroFile Destination { get; }

    public IObservable<IProgress> Progress => progressSubject.AsObservable();

    public Task<Result> Sync(CancellationToken cancellationToken)
    {
        return Source.Copy(Destination, Maybe<IObserver<LongProgress>>.From(progressSubject), cancellationToken: cancellationToken);
    }
}