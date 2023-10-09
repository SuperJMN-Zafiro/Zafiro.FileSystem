using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.FileSystem.Comparer;

namespace Zafiro.FileSystem.Actions;

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
                    .Select(leftDiff => leftDiff.Left.Translate(source, destination)
                        .Bind(dest => CopyFileAction.Create(leftDiff.Left, dest)));
                var whenAll = await Task.WhenAll(tasks);
                var combine = whenAll.Combine();
                return combine;
            });

        return childActions.Map(r => new CopyLeftFilesToRightSideAction(r.Cast<IFileAction>().ToList()));
    }
}
