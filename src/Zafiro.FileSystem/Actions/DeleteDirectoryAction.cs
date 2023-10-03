using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Actions;

public class DeleteDirectoryAction : IAction<LongProgress>
{
    private readonly CompositeAction compositeAction;

    private DeleteDirectoryAction(IZafiroDirectory source, CompositeAction compositeAction)
    {
        Source = source;
        this.compositeAction = compositeAction;
        Progress = compositeAction.Progress;
    }

    public IZafiroDirectory Source { get; }

    public IObservable<LongProgress> Progress { get; }

    public Task<Result> Execute(CancellationToken ct)
    {
        return compositeAction.Execute(ct);
    }

    public static async Task<Result<DeleteDirectoryAction>> Create(IZafiroDirectory source)
    {
        var files = await source.GetFilesInTree().ConfigureAwait(false);

        var action = await files.Map(async zafiroFiles =>
        {
            var childrenTasks = await GetChildrenTasks(zafiroFiles).ToList();
            return new CompositeAction(childrenTasks);
        }).ConfigureAwait(false);

        return action.Map(compositeAction => new DeleteDirectoryAction(source, compositeAction));
    }

    private static IObservable<IAction<LongProgress>> GetChildrenTasks(IEnumerable<IZafiroFile> sources)
    {
        var results = sources
            .ToObservable()
            .Select(DeleteFileAction.Create).Successes();

        return results;
    }
}