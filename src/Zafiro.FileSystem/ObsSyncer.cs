using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.Core.Mixins;
using Zafiro.FileSystem.ZafiroCoreCandidates;

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
            FileDiffStatus.Both => Observable.Return(new NoopAction()),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static IObservable<ISyncAction> Return(Diff diff, IZafiroDirectory source, IZafiroDirectory destination)
    {
        var getSource = () => source.GetFile(source.Path.Combine(diff.Path));
        var getDestination = () => destination.GetFile(destination.Path.Combine(diff.Path));

        return getSource.Combine(getDestination, (o, d) => new CopyAction(o, d))
            .WhereSuccess();
    }
}

internal class NoopAction : ISyncAction
{
    public IZafiroFile Source { get; set; }
    public IZafiroFile Destination { get; set; }
    public IObservable<RelativeProgress<long>> Progress => Observable.Return(new RelativeProgress<long>(1, 1));
    public IObservable<Result> Sync()
    {
        return Observable.Return(Result.Success());
    }
}
