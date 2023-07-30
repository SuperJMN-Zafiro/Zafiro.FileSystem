using System.Reactive.Linq;
using JetBrains.Annotations;

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
            FileDiffStatus.RightOnly => Observable.Return<ISyncAction>(new DeleteAction(source, diff.Path)),
            FileDiffStatus.LeftOnly => Observable.Return<ISyncAction>(new CopyAction(source, diff.Path, destination)),
            FileDiffStatus.Invalid => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}