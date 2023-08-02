using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.Zafiro.Functional;

namespace Zafiro.FileSystem;

[PublicAPI]
public class ObsSyncer
{
    public IObservable<ISyncAction> Sync(IZafiroDirectory source, IZafiroDirectory destination, IEnumerable<Diff> diffs)
    {
        return SyncItems(source, destination, diffs);
    }

    private IObservable<ISyncAction> SyncItems(IZafiroDirectory source, IZafiroDirectory destination, IEnumerable<Diff> diffs)
    {
        return diffs.ToObservable()
            .Select(diff => Sync(diff, source, destination))
            .Merge();
    }

    private IObservable<ISyncAction> Sync(Diff diff, IZafiroDirectory source, IZafiroDirectory destination)
    {
        return diff.Status switch
        {
            FileDiffStatus.RightOnly => DeleteAction(diff, source),
            FileDiffStatus.LeftOnly => CopyAction(diff, source, destination),
            FileDiffStatus.Both => CopyAction(diff, source, destination),
            FileDiffStatus.Invalid => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public IObservable<ISyncAction> DeleteAction(Diff diff, IZafiroDirectory source)
    {
        var action = Observable
            .FromAsync(() => source.GetFile(source.Path.Combine(diff.Path)))
            .Select(result => result.Map(f => new DeleteAction(f)))
            .Successes();

        return action;
    }

    private static IObservable<ISyncAction> CopyAction(Diff diff, IZafiroDirectory source, IZafiroDirectory destination)
    {
        var getSource = () => source.GetFile(source.Path.Combine(diff.Path));
        var getDestination = () => destination.GetFile(destination.Path.Combine(diff.Path));

        return getSource.Combine(getDestination, (o, d) => new CopyAction(o, d))
            .Successes();
    }

    private static IObservable<ISyncAction> DeleteAction(Diff diff, IZafiroDirectory source, IZafiroDirectory destination)
    {
        var getSource = () => source.GetFile(source.Path.Combine(diff.Path));
        var getDestination = () => destination.GetFile(destination.Path.Combine(diff.Path));

        return getSource.Combine(getDestination, (o, d) => new CopyAction(o, d))
            .Successes();
    }
}