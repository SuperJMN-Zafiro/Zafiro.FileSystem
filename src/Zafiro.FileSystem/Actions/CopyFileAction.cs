using CSharpFunctionalExtensions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Zafiro.Actions;

namespace Zafiro.FileSystem.Actions;

public class CopyFileAction : IAction<LongProgress>
{
    private readonly BehaviorSubject<LongProgress> progress = new(new LongProgress());

    public CopyFileAction(IZafiroFile source, IZafiroFile destination)
    {
        Source = source;
        Destination = destination;
    }

    public IZafiroFile Source { get; }
    public IZafiroFile Destination { get; }

    public IObservable<LongProgress> Progress => progress.AsObservable();

    public Task<Result> Execute(CancellationToken ct)
    {
        return Source.Copy(Destination, Maybe<IObserver<LongProgress>>.From(progress), cancellationToken: ct);
    }
}