using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Comparer;

namespace Zafiro.FileSystem.Synchronizer;

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
                    .Select(diff =>
                    {
                        var valueTuple = FileSystem.DiffExtensions.Get(diff, source, destination);
                        return CopyFileAction.Create(valueTuple.Item1, valueTuple.Item2);
                    });
                var whenAll = await Task.WhenAll(tasks);
                var combine = whenAll.Combine();
                return combine;
            });

        return childActions.Map(r => new OverwriteRightFilesInBothSides(r.Cast<IFileAction>().ToList()));
    }
}
