using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Serilog;
using Zafiro.Mixins;
using Zafiro.ProgressReporting;

namespace Zafiro.FileSystem;

[PublicAPI]
[Obsolete("Use Syncer instead")]
public class OldSyncer
{
    private readonly Maybe<ILogger> logger;
    
    public OldSyncer(Maybe<ILogger> logger)
    {
        this.logger = logger;
        Operations = new Subject<CopyOperation>();
        Progress = new Subject<RelativeProgress<int>>();
    }

    public Subject<CopyOperation> Operations { get; }

    public TimeSpan? ReadTimeout { get; set; }

    public ISubject<RelativeProgress<int>> Progress { get; }

    public Task<Result> Sync(IZafiroDirectory source, IZafiroDirectory destination, IEnumerable<Diff> diffs)
    {
        return SyncItems(source, destination, diffs);
    }

    private static Task<Result> DeleteNonexistent(IZafiroDirectory origin, ZafiroPath path)
    {
        return Task.FromResult(Result.Failure("DeleteNonexistent: Not implemented, colleiga"));
    }

    private async Task<Result> CopyOverExisting(IZafiroDirectory sourceDirectory, IZafiroDirectory destinationDirectory, ZafiroPath path)
    {
        var areSame = true;
        if (areSame)
        {
            return Result.Success();
        }

        return await CopyNonexistent(sourceDirectory, destinationDirectory, path);
    }

    private async Task<Result> CopyWithRetries(IZafiroFile sourceFile, IZafiroFile destination)
    {
        var observable = new Subject<double>();
        var copyOp = new CopyOperation(sourceFile, observable);
        Operations.OnNext(copyOp);

        var copyTask = Observable
            .FromAsync(() => sourceFile.Copy(destination, observable, readTimeout: TimeSpan.FromSeconds(0.5)))
            .RetryWithBackoffStrategy();

        var copyResult = await copyTask;
        return copyResult;
    }

    private async Task<Result> SyncItems(IZafiroDirectory source, IZafiroDirectory destination, IEnumerable<Diff> diffs)
    {
        var list = diffs.ToList();
        var processed = 0;
        var results = await list.Select(diff => Observable.FromAsync(() => Sync(diff, source, destination)))
            .Merge(3)
            .Do(_ => processed++)
            .Do(_ => Progress.OnNext(new RelativeProgress<int>(list.Count, processed)))
            .ToList();

        return results.Combine();
    }

    private async Task<Result> Sync(Diff diff, IZafiroDirectory source, IZafiroDirectory destination)
    {
        switch (diff.Status)
        {
            case FileDiffStatus.RightOnly:
                logger.Execute(l => l.Information("Deleting {File} {Status}", diff.Path, "non-existent in origin"));
                await DeleteNonexistent(source, diff.Path).TapError(msg => logger.Execute(l => l.Error("{File} could not be deleted: {ErrorMessage}", source, msg)));
                break;
            case FileDiffStatus.Both:
                logger.Execute(l => l.Information("Copying {File} {Status}", diff.Path, "existent in both"));
                await CopyOverExisting(source, destination, diff.Path).TapError(msg => logger.Execute(l => l.Error("{File} could not be copied: {ErrorMessage}", source, msg)));
                break;
            case FileDiffStatus.LeftOnly:
                logger.Execute(l => l.Information("Copying {File} {Status}", diff.Path, "non-existent in destination"));
                await CopyNonexistent(source, destination, diff.Path).TapError(msg => logger.Execute(l => l.Error("{File} could not be copied: {ErrorMessage}", source, msg)));
                break;
            default:
                return Result.Failure("Invalid status");
        }

        return Result.Success();
    }

    private async Task<Result> CopyNonexistent(IZafiroDirectory sourceDirectory, IZafiroDirectory destinationDirectory, ZafiroPath path)
    {
        var sourceFile = await sourceDirectory.GetFile(sourceDirectory.Path.Combine(path));
        var destination = await destinationDirectory.GetFile(destinationDirectory.Path.Combine(path));

        //var copyResultTask = from s in sourceFile
        //    from d in destination
        //    select CopyWithRetries(s, d);

        var copyResult = await sourceFile
            .Bind(source => destination.Map(destination => (source, destination)))
            .Bind(x => CopyWithRetries(x.source, x.destination));

        return copyResult;
    }
}