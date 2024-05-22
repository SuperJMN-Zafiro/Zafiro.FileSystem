using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.NewComparer;
using Zafiro.FileSystem.Lightweight;
using Zafiro.Mixins;

namespace AvaloniaSyncer.Console;

public class Syncer
{
    private readonly Maybe<ILogger> logger;

    public Syncer(Maybe<ILogger> logger)
    {
        this.logger = logger;
    }

    public async Task<Result> Sync(FileSource left, FileSource right)
    {
        logger.Execute(l => l.Information("Syncing {Left} and {Right}", left, right));

        var leftResult = await left.GetFiles();
        var rightResult = await right.GetFiles();

        return from l in leftResult from r in rightResult select Sync(l, r);
    }

    private async Task<Result> Sync(IDirectory left, IDirectory right)
    {
        var leftFiles = left.FilesInTree(ZafiroPath.Empty);
        var rightFiles = right.FilesInTree(ZafiroPath.Empty);
        var diffs = leftFiles.Diff(rightFiles);
        logger.Execute(l => l.Information("Diffs {Diffs}", diffs.JoinWithLines()));

        return await diffs.Select(ProcessDiff).Combine();
        
        return Result.Success();
    }

    private async Task<Result> ProcessDiff(FileDiff diff)
    {
        switch (diff)
        {
            case BothDiff bothDiff:
                break;
            case LeftOnlyDiff leftOnlyDiff:
                return await LeftOnly(leftOnlyDiff);
                break;
            case RightOnlyDiff rightOnlyDiff:
                return await RightOnly(rightOnlyDiff);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(diff));
        }

        return Result.Success();
    }

    private async Task<Result> LeftOnly(LeftOnlyDiff leftOnlyDiff)
    {
        logger.Execute(l => l.Information("Copy Left: {ToRight} to Right: ??", leftOnlyDiff.Path));
        return Result.Success();
    }
    
    private async Task<Result> RightOnly(RightOnlyDiff rightOnlyDiff)
    {
        logger.Execute(l => l.Information("Delete Right: {ToRight}", rightOnlyDiff.Path));
        return Result.Success();
    }
}