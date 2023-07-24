using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.FileSystem;

public class Syncer
{
    private readonly ZafiroFileSystemComparer comparer;
    private readonly Maybe<ILogger> logger;
    private readonly Subject<CopyOperation> copies;
    private readonly ISubject<RelativeProgress<int>> progress;

    public Syncer(ZafiroFileSystemComparer systemComparer, Maybe<ILogger> logger)
    {
        comparer = systemComparer;
        this.logger = logger;
        copies = new Subject<CopyOperation>();
        progress = new Subject<RelativeProgress<int>>();
    }

    public Subject<CopyOperation> Operations => copies;
    public IObservable<RelativeProgress<int>> Progress => progress;

    public async Task<Result> Sync(IZafiroDirectory source, IZafiroDirectory destination)
    {
        var diffsResult = await comparer.Diff(source, destination).ConfigureAwait(false);
        return await diffsResult.Bind(diffs => SyncItems(source, destination, diffs));
    }

    private async Task<Result> SyncItems(IZafiroDirectory source, IZafiroDirectory destination, IEnumerable<ZafiroFileDiff> diffs)
    {
        var list = diffs.ToList();
        var processed = 0;
        var results = await list.Select(diff => Observable.FromAsync(() => Sync(diff, source, destination)))
            .Merge(3)
            .Do(_ => processed++)
            .Do(_ => progress.OnNext(new RelativeProgress<int>(list.Count, processed))  )
            .ToList();

        return results.Combine();
    }

    private static async Task<Result> DeleteNonexistent(IZafiroFile file)
    {
        //return await file.Delete();
        return Result.Failure("DeleteNonexistent: Not implemented, colleiga");
    }

    private static async Task<Result> CopyOverExisting(IZafiroFile source, IZafiroFile destination)
    {
        var areSame = true;
        if (areSame)
        {
            return Result.Success();
        }

        return await source.Copy(destination, Maybe<IObserver<double>>.None);
    }

    private async Task<Result> Sync(ZafiroFileDiff diff, IZafiroDirectory source, IZafiroDirectory destination)
    {
        switch (diff.Status)
        {
            case FileDiffStatus.RightOnly:
                logger.Execute(l => l.Information("Deleting {File} {Status}", diff.Right.Value, "non-existent in origin"));
                await DeleteNonexistent(diff.Right.Value).TapError(e => logger.Execute(l => l.Error(e)));
                break;
            case FileDiffStatus.Both:
                logger.Execute(l => l.Information("Copying {File} {Status}", diff.Left.Value, "existent in both"));
                await CopyOverExisting(diff.Left.Value, diff.Right.Value).TapError(e => logger.Execute(l => l.Error(e)));
                break;
            case FileDiffStatus.LeftOnly:
                logger.Execute(l => l.Information("Copying {File} {Status}", diff.Left.Value, "non-existent in destination"));
                await CopyNonexistent(source, destination, diff.Left.Value)
                    .TapError(e => logger.Execute(l => l.Error(e)));
                break;
            default:
                return Result.Failure("Invalid status");
        }

        return Result.Success();
    }

    private async Task<Result> CopyNonexistent(IZafiroDirectory sourceDirectory,
        IZafiroDirectory destinationDirectory, IZafiroFile sourceFile)
    {
        var destPath = destinationDirectory.Path.Combine(sourceFile.Path.MakeRelativeTo(sourceDirectory.Path));
        var destination = await destinationDirectory.GetFile(destPath);
        ISubject<double> partialProgress = new Subject<double>();
        copies.OnNext(new CopyOperation(sourceFile, partialProgress));
        var result = await destination.Bind(f => sourceFile.Copy(f, Maybe<IObserver<double>>.From(partialProgress)));
        return result;
    }
}