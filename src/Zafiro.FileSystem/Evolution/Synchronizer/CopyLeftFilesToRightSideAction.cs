using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Evolution.Comparer;
using CopyFileAction = Zafiro.FileSystem.Evolution.Actions.CopyFileAction;

namespace Zafiro.FileSystem.Evolution.Synchronizer;

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

    public static async Task<Result<CopyLeftFilesToRightSideAction>> Create(IZafiroDirectory2 source, IZafiroDirectory2 destination)
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
    public static IZafiroFile2 Get(this FileDiff diff, IZafiroDirectory2 origin)
    {
        return origin.FileSystem.GetFile(origin.Path.Combine(diff.Path));
    }

    public static Task<Result<(IZafiroFile2, IZafiroFile2)>> Get(this FileDiff diff, IZafiroDirectory2 origin, IZafiroDirectory2 destination)
    {
        return Task.FromResult(Result.Success((diff.Get(origin), diff.Get(destination))));
    }
}