using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Comparer;
using CopyFileAction = Zafiro.FileSystem.Actions.CopyFileAction;

namespace Zafiro.FileSystem.Synchronizer;

public class CopyLeftFilesToRightSideAction : IFileAction
{
    private readonly CompositeAction composite;

    private CopyLeftFilesToRightSideAction(IEnumerable<IFileAction> copyFileActions)
    {
        composite = new CompositeAction(copyFileActions.Cast<IAction<LongProgress>>().ToList());
        Progress = composite.Progress;
    }

    public IObservable<LongProgress> Progress { get; }
    public Task<Result> Execute(CancellationToken cancellationToken)
    {
        return composite.Execute(cancellationToken);
    }

    public static async Task<Result<CopyLeftFilesToRightSideAction>> Create(IZafiroDirectory source, IZafiroDirectory destination)
    {
        Result<IEnumerable<CopyFileAction>> childActions = await new FileSystemComparer().Diff(source, destination)
            .Bind(async diffs =>
            {
                var tasks = diffs.OfType<LeftOnly>()
                    .Select(leftDiff => leftDiff.Get(source, destination)
                        .Bind(dirs => CopyFileAction.Create(dirs.Item1, dirs.Item2)));
                var whenAll = await Task.WhenAll(tasks);
                var combine = whenAll.Combine();
                return combine;
            });

        return childActions.Map(r => new CopyLeftFilesToRightSideAction(r.Cast<IFileAction>().ToList()));
    }
}


public static class DiffExtensions
{
    public static IZafiroFile Get(this FileDiff diff, IZafiroDirectory origin)
    {
        return origin.FileSystem.GetFile(origin.Path.Combine(diff.Path));
    }

    public static Task<Result<(IZafiroFile, IZafiroFile)>> Get(this FileDiff diff, IZafiroDirectory origin, IZafiroDirectory destination)
    {
        return Task.FromResult(Result.Success((diff.Get(origin), diff.Get(destination))));
    }
}