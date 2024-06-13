using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.Actions;

public class CopyFileAction : IFileAction
{
    private readonly BehaviorSubject<LongProgress> progress;
    private readonly TimeSpan? readTimeout;
    private readonly IScheduler? timeoutScheduler;
    private readonly IScheduler? progressScheduler;

    public CopyFileAction(IData source, IMutableFile destination)
    {
        Source = source;
        Destination = destination;
        progress = new BehaviorSubject<LongProgress>(new LongProgress(0, source.Length));
    }

    public IData Source { get; }
    public IMutableFile Destination { get; }

    public IObservable<LongProgress> Progress => progress.AsObservable();

    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        var progressObserver = new Subject<long>();
        using var longProgressSubscription = progressObserver.Select(l => new LongProgress(l, Source.Length)).Subscribe(progress);
        using (new ProgressWatcher(Source, progressObserver))
        {
            var result = Destination.SetContents(Source, cancellationToken);
            return result;
        }
    }
}