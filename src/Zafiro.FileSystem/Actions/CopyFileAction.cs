using CSharpFunctionalExtensions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Zafiro.Actions;

namespace Zafiro.FileSystem.Actions;

public class CopyFileAction : IAction<LongProgress>
{
    private readonly BehaviorSubject<LongProgress> progress;

    public CopyFileAction(IZafiroFile source, IZafiroFile destination, long fileSize)
    {
        Source = source;
        Destination = destination;
        progress = new BehaviorSubject<LongProgress>(new LongProgress(0, fileSize));
    }

    public IZafiroFile Source { get; }
    public IZafiroFile Destination { get; }

    public IObservable<LongProgress> Progress => progress.AsObservable();

    public Task<Result> Execute(CancellationToken ct)
    {
        return Source.Copy(Destination, Maybe<IObserver<LongProgress>>.From(progress), cancellationToken: ct);
    }

    public static Task<Result<CopyFileAction>> Create(IZafiroFile source, IZafiroFile destination)
    {
        return source.Size().Map(l => new CopyFileAction(source, destination, l));
    }
}