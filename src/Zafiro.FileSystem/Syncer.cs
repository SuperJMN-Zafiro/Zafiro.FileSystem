using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Serilog;
using Zafiro.Core.Mixins;

namespace Zafiro.FileSystem;

[PublicAPI]
public class Syncer
{
    private readonly Maybe<ILogger> logger;
    private readonly ISubject<RelativeProgress<int>> progress;

    public Syncer(Maybe<ILogger> logger)
    {
        this.logger = logger;
        Operations = new Subject<CopyOperation>();
        progress = new Subject<RelativeProgress<int>>();
    }

    public Subject<CopyOperation> Operations { get; }

    public TimeSpan? ReadTimeout { get; set; }

    public IObservable<RelativeProgress<int>> Progress => progress;

    public async Task<Result> Sync(IZafiroDirectory source, IZafiroDirectory destination, IEnumerable<Diff> diffs)
    {
        return await SyncItems(source, destination, diffs);
    }

    private static async Task<Result> DeleteNonexistent(IZafiroDirectory origin, ZafiroPath path)
    {
        //return await file.Delete();
        return Result.Failure("DeleteNonexistent: Not implemented, colleiga");
    }

    private static async Task<Result> CopyOverExisting(IZafiroDirectory sourceDirectory, IZafiroDirectory destinationDirectory, ZafiroPath path)
    {
        var areSame = true;
        if (areSame)
        {
            return Result.Success();
        }

        return await CopyNonexistent(sourceDirectory, destinationDirectory, path);
    }

    private static IObservable<Result> CopyWithRetries(IZafiroFile sourceFile, IZafiroFile x)
    {
        return Observable
            .FromAsync(() => x.Copy(sourceFile, Maybe<IObserver<double>>.None, readTimeout: TimeSpan.FromSeconds(15)))
            .RetryWithBackoffStrategy();
    }

    private async Task<Result> SyncItems(IZafiroDirectory source, IZafiroDirectory destination, IEnumerable<Diff> diffs)
    {
        var list = diffs.ToList();
        var processed = 0;
        var results = await list.Select(diff => Observable.FromAsync(() => Sync(diff, source, destination)))
            .Merge(3)
            .Do(_ => processed++)
            .Do(_ => progress.OnNext(new RelativeProgress<int>(list.Count, processed)))
            .ToList();

        return results.Combine();
    }

    private async Task<Result> Sync(Diff diff, IZafiroDirectory source, IZafiroDirectory destination)
    {
        switch (diff.Status)
        {
            case FileDiffStatus.RightOnly:
                logger.Execute(l => l.Information("Deleting {File} {Status}", diff.Path, "non-existent in origin"));
                await DeleteNonexistent(source, diff.Path).TapError(e => logger.Execute(l => l.Error(e)));
                break;
            case FileDiffStatus.Both:
                logger.Execute(l => l.Information("Copying {File} {Status}", diff.Path, "existent in both"));
                await CopyOverExisting(source, destination, diff.Path).TapError(e => logger.Execute(l => l.Error(e)));
                break;
            case FileDiffStatus.LeftOnly:
                logger.Execute(l => l.Information("Copying {File} {Status}", diff.Path, "non-existent in destination"));
                await CopyNonexistent(source, destination, diff.Path)
                    .TapError(e => logger.Execute(l => l.Error(e)));
                break;
            default:
                return Result.Failure("Invalid status");
        }

        return Result.Success();
    }

    private static async Task<Result> CopyNonexistent(IZafiroDirectory sourceDirectory, IZafiroDirectory destinationDirectory, ZafiroPath path)
    {
        var sourceFile = await sourceDirectory.GetFile(sourceDirectory.Path.Combine(path));
        var destination = await destinationDirectory.GetFile(destinationDirectory.Path.Combine(path));

        return from s in sourceFile
            from d in destination
            select CopyWithRetries(s, d).ToTask();
    }
}