using CSharpFunctionalExtensions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Zafiro.Actions;

namespace Zafiro.FileSystem.Actions;

public class CopyFileAction : IAction
{
    private readonly BehaviorSubject<IProportionProgress> progress = new(new ProportionProgress());

    public CopyFileAction(IZafiroFile source, IZafiroFile destination)
    {
        Source = source;
        Destination = destination;
    }

    public IZafiroFile Source { get; }
    public IZafiroFile Destination { get; }

    public IObservable<IProportionProgress> Progress => progress.AsObservable();

    public Task<Result> Execute(CancellationToken ct)
    {
        return Source.Copy(Destination, Maybe<IObserver<IProportionProgress>>.From(progress), cancellationToken: ct);
    }
}