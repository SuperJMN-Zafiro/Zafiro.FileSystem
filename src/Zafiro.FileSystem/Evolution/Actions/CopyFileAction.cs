using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.FileSystem.Actions;

namespace Zafiro.FileSystem.Evolution.Actions;

public class CopyFileAction : IFileAction
{
    private readonly BehaviorSubject<LongProgress> progress;

    public CopyFileAction(IZafiroFile2 source, IZafiroFile2 destination, long fileSize)
    {
        Source = source;
        Destination = destination;
        progress = new BehaviorSubject<LongProgress>(new LongProgress(0, fileSize));
    }

    public IZafiroFile2 Source { get; }
    public IZafiroFile2 Destination { get; }

    public IObservable<LongProgress> Progress => progress.AsObservable();

    public Task<Result> Execute(CancellationToken cancellationToken)
    {
        return Source.Copy(Destination, Maybe<IObserver<LongProgress>>.From(progress), cancellationToken: cancellationToken);
    }

    public static Task<Result<CopyFileAction>> Create(IZafiroFile2 source, IZafiroFile2 destination)
    {
        return source.Properties.Map(p => new CopyFileAction(source, destination, p.Length));
    }
}