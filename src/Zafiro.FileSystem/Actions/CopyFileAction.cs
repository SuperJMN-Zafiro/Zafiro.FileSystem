using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Actions;

namespace Zafiro.FileSystem.Actions;

public class CopyFileAction : IFileAction
{
    private readonly BehaviorSubject<LongProgress> progress;
    private readonly TimeSpan? readTimeout;
    private readonly IScheduler? timeoutScheduler;
    private IScheduler? progressScheduler;

    public CopyFileAction(IZafiroFile source, IZafiroFile destination, long fileSize, IScheduler? timeoutScheduler = default, IScheduler? progressScheduler = default, TimeSpan? readTimeout = default)
    {
        Source = source;
        Destination = destination;
        this.timeoutScheduler = timeoutScheduler;
        this.progressScheduler = progressScheduler;
        this.readTimeout = readTimeout;
        progress = new BehaviorSubject<LongProgress>(new LongProgress(0, fileSize));
    }

    public IZafiroFile Source { get; }
    public IZafiroFile Destination { get; }

    public IObservable<LongProgress> Progress => progress.AsObservable();

    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        return Source.Copy(Destination, Maybe<IObserver<LongProgress>>.From(progress), progressScheduler, timeoutScheduler, readTimeout: readTimeout, cancellationToken: cancellationToken);
    }

    public static Task<Result<CopyFileAction>> Create(IZafiroFile source, IZafiroFile destination)
    {
        return source.Properties.Map(p => new CopyFileAction(source, destination, p.Length));
    }
}