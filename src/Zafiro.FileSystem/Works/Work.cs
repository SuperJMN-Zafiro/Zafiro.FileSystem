using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.DataModel;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.Readonly;
using Zafiro.Progress;
using Zafiro.Works;

namespace Zafiro.FileSystem.Works;

public class Work : IWork
{
    public IFile File { get; }
    public IMutableDirectory Directory { get; }
    public Maybe<ILogger> Logger { get; }
    public IScheduler? Scheduler { get; }
    
    private readonly BehaviorSubject<IProgress> progressSubject;

    public Work(IFile file, IMutableDirectory directory, Maybe<ILogger> logger, IScheduler? scheduler)
    {
        File = file;
        Directory = directory;
        Logger = logger;
        Scheduler = scheduler ?? System.Reactive.Concurrency.Scheduler.Default;
        progressSubject = new BehaviorSubject<IProgress>(new ProgressWithCurrentAndTotal<long>(0, file.Length));
    }

    public IObservable<Result> Execute()
    {
        return Observable.FromAsync(ct => CopyAndPreserveExisting(File, Directory, Scheduler, ct));
    }

    private async Task<Result> CopyAndPreserveExisting(IFile file, IMutableDirectory directory, IScheduler? scheduler, CancellationToken cancellationToken)
    {
        var longSubject = new Subject<long>();
        var progress = longSubject.Select(l => new ProgressWithCurrentAndTotal<long>(l, file.Length));
        using (progress.Subscribe(progressSubject))
        {
            using (new ProgressWatcher(file, longSubject))
            {
                return await file.SafeCopy(directory, Logger, scheduler: scheduler, cancellationToken: cancellationToken);
            }    
        }
    }
    
    public IObservable<IProgress> Progress => progressSubject.AsObservable();
}