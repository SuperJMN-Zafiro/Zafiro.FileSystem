using System.Diagnostics;
using AvaloniaSyncer.Console.Core;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.NewComparer;

namespace AvaloniaSyncer.Console;

public class Syncer
{
    private readonly Maybe<ILogger> logger;

    public Syncer(Maybe<ILogger> logger)
    {
        this.logger = logger;
    }

    public Task<Result> Sync(FileSource left, FileSource right)
    {
        Debugger.Launch();
        logger.Execute(l => l.Information("Syncing {Left} and {Right}", left, right));

        var leftResult = left.GetFiles().Map(x => x.RootedFiles());
        var rightResult = right.GetFiles().Map(x => x.RootedFiles());

        return leftResult
            .CombineAndMap(rightResult, (l, r) => l.Diff(r))
            .Bind(s => s.Select(diff => ProcessDiff(diff, left, right)).Combine());
    }

    private Task<Result> ProcessDiff(FileDiff diff, FileSource leftPath, FileSource rightPath)
    {
        switch (diff)
        {
            case BothDiff bothDiff:
                return OnBothDiff(bothDiff, leftPath, rightPath);
            case LeftOnlyDiff leftOnlyDiff:
                return LeftOnly(leftOnlyDiff, leftPath, rightPath);
            case RightOnlyDiff rightOnlyDiff:
                return RightOnly(rightOnlyDiff, leftPath, rightPath);
            default:
                throw new ArgumentOutOfRangeException(nameof(diff));
        }
    }

    private async Task<Result> OnBothDiff(BothDiff bothDiff, FileSource leftPath, FileSource rightPath)
    {
        //logger.Execute(l => l.Information("Both: {Left} || {Right} => {Name}", leftPath, rightPath, bothDiff.Left.Name));
        return Result.Success();
    }

    private async Task<Result> LeftOnly(LeftOnlyDiff leftOnlyDiff, FileSource leftSource, FileSource rightSource)
    {
        var result = await rightSource.Plugin.Copy(leftOnlyDiff.Left, rightSource.Path.Combine(leftOnlyDiff.Left.FullPath()));
        logger.Execute(l => l.Information("Left only: {Left} => {Name}", leftSource, leftOnlyDiff.Left.Name));
        result.TapError(err => logger.Execute(l => l.Error(err)));
        return result;
    }
    
    private async Task<Result> RightOnly(RightOnlyDiff rightOnlyDiff, FileSource leftPath, FileSource rightPath)
    {
        logger.Execute(l => l.Information("Right only: {Right} => {Name}", rightPath, rightOnlyDiff.Right));
        return Result.Success();
    }
}