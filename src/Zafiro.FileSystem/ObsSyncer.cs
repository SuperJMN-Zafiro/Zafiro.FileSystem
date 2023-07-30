using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.Core.Mixins;

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
            FileDiffStatus.LeftOnly => Return(diff, source, destination),
            FileDiffStatus.Invalid => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static IObservable<ISyncAction> Return(Diff diff, IZafiroDirectory source, IZafiroDirectory destination)
    {
        var r = from s in Observable.FromAsync(() => source.GetFile(source.Path.Combine(diff.Path)))
            from n in Observable.FromAsync(() => destination.GetFile(destination.Path.Combine(diff.Path)))
            select new{ s, n};

        var action = r.Select(arg => from n in arg.s from q in arg.n select new CopyAction(n, q));

        return action.WhereSuccess();
    }
}