using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Comparer;

namespace Zafiro.FileSystem.Actions;

// TODO: Test this
public class OverwriteRightFilesInBothSides : IFileAction
{
    private readonly CompositeAction composite;

    private OverwriteRightFilesInBothSides(IEnumerable<IFileAction> copyFileActions)
    {
        composite = new CompositeAction(copyFileActions.Cast<IAction<LongProgress>>().ToList());
        Progress = composite.Progress;
    }

    public IObservable<LongProgress> Progress { get; }
    public Task<Result> Execute(CancellationToken cancellationToken)
    {
        return composite.Execute(cancellationToken);
    }

    public static async Task<Result<OverwriteRightFilesInBothSides>> Create(IZafiroDirectory source, IZafiroDirectory destination)
    {
        Result<IEnumerable<CopyFileAction>> childActions = await new FileSystemComparer().Diff(source, destination)
            .Bind(async diffs =>
            {
                var tasks = diffs.OfType<Both>()
                    .Select(bothDiff => CopyFileAction.Create(bothDiff.Left, bothDiff.Right));
                var whenAll = await Task.WhenAll(tasks);
                var combine = whenAll.Combine();
                return combine;
            });

        return childActions.Map(actions => new OverwriteRightFilesInBothSides(actions.Cast<IFileAction>().ToList()));
    }
}

public class SyncOptions
{
    public SyncOptions(IStrategy bothStrategy, IStrategy leftOnlyStrategy, IStrategy rightOnlyStrategy)
    {
        BothStrategy = bothStrategy;
        LeftOnlyStrategy = leftOnlyStrategy;
        RightOnlyStrategy = rightOnlyStrategy;
    }

    public IStrategy BothStrategy { get; }
    public IStrategy LeftOnlyStrategy { get; }
    public IStrategy RightOnlyStrategy { get; }
}

public interface IStrategy
{
    Task<Result<IFileAction>> Create(IZafiroDirectory source, IZafiroDirectory destination);
}

public class OverwriteRightFilesInBothSidesStrategy : IStrategy
{
    public Task<Result<IFileAction>> Create(IZafiroDirectory source, IZafiroDirectory destination)
    {
        return OverwriteRightFilesInBothSides.Create(source, destination).Cast(r => (IFileAction)r);
    }
}

public class CopyLeftFilesStrategy : IStrategy
{
    public Task<Result<IFileAction>> Create(IZafiroDirectory source, IZafiroDirectory destination)
    {
        return CopyLeftFilesToRightSideAction.Create(source, destination).Cast(r => (IFileAction)r);
    }
}

public class DeleteFilesOnlyOnRightSideStrategy : IStrategy
{
    public Task<Result<IFileAction>> Create(IZafiroDirectory source, IZafiroDirectory destination)
    {
        return DeleteFilesOnlyOnRightSide.Create(source, destination).Cast(r => (IFileAction)r);
    }
}