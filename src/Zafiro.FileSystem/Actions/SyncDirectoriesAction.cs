using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Actions;

public class SyncDirectoriesAction : IFileAction
{
    private readonly CompositeAction syncAction;

    private SyncDirectoriesAction(CompositeAction syncAction)
    {
        this.syncAction = syncAction;
        Progress = syncAction.Progress;
    }

    public IObservable<LongProgress> Progress { get; }

    public Task<Result> Execute(CancellationToken cancellationToken)
    {
        return syncAction.Execute(cancellationToken);
    }

    public static async Task<SyncDirectoriesAction> Create(IEnumerable<FileDiff> fileDiff)
    {
        var subActions = await fileDiff.ToObservable()
            .Select(diff => Observable.FromAsync(() => GetAction(diff)))
            .Merge()
            .Values().ToList();

        var syncAction = new CompositeAction(subActions);
        return new SyncDirectoriesAction(syncAction);
    }

    private static Task<Maybe<IAction<LongProgress>>> GetAction(FileDiff diff)
    {
        return diff switch
        {
            LeftOnlyDiff leftOnlyDiff => leftOnlyDiff.LeftFile().CombineAndBind(leftOnlyDiff.RightFile(), (a, b) => CopyFileAction.Create(a, b).Cast<CopyFileAction, IAction<LongProgress>>()).AsMaybe(),
            RightOnlyDiff rightOnlyDiff => Task.FromResult(Maybe<IAction<LongProgress>>.None),
            BothSidesDiff bothSidesDiff => Task.FromResult(Maybe<IAction<LongProgress>>.None),
            _ => throw new ArgumentOutOfRangeException(nameof(diff))
        };
    }
}

public abstract class FileDiff
{
    public ZafiroPath Path { get; }
    private IZafiroDirectory Source { get; }
    private IZafiroDirectory Destination { get; }
    public Task<Result<IZafiroFile>> LeftFile()
    {
        return Source.DescendantFile(Path);
    }

    public Task<Result<IZafiroFile>> RightFile()
    {
        return Destination.DescendantFile(Path);
    }

    public FileDiff(IZafiroDirectory source, IZafiroDirectory destination, ZafiroPath path)
    {
        Source = source;
        Destination = destination;
        Path = path;
    }
}

public class LeftOnlyDiff : FileDiff
{
    public LeftOnlyDiff(IZafiroDirectory source, IZafiroDirectory destination, ZafiroPath path) : base(source, destination, path)
    {
    }
}

public class RightOnlyDiff : FileDiff
{
    public RightOnlyDiff(IZafiroDirectory source, IZafiroDirectory destination, ZafiroPath path) : base(source, destination, path)
    {
    }
}

public class BothSidesDiff : FileDiff
{
    public BothSidesDiff(IZafiroDirectory source, IZafiroDirectory destination, ZafiroPath path) : base(source, destination, path)
    {
    }
}