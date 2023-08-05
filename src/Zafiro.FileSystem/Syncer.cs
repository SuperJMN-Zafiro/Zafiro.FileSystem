using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.Functional;

namespace Zafiro.FileSystem;

[PublicAPI]
public class Syncer
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
            FileDiffStatus.RightOnly => OnRightOnly(diff, source),
            FileDiffStatus.LeftOnly => OnLeftOnly(diff, source, destination),
            FileDiffStatus.Both => OnBoth(diff, source, destination),
            FileDiffStatus.Invalid => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private IObservable<ISyncAction> OnBoth(Diff diff, IZafiroDirectory source, IZafiroDirectory destination)
    {
        //TODO refactor this
        //var srcObs = GetFileEntryData(diff, source);
        //var dstObs = GetFileEntryData(diff, destination);
        
        var getSource = Observable.FromAsync(() => source.GetFile(source.Path.Combine(diff.Path)));
        var getDestination = Observable.FromAsync(() => destination.GetFile(destination.Path.Combine(diff.Path)));

        return getSource
            .Zip(getDestination, (s, d) => s.Combine(d, (src, dst) => (ISyncAction)new CopyAction(src, dst)))
            .SelectMany(result => result.IsSuccess ? Observable.Return(result.Value) : Observable.Empty<ISyncAction>());
    }

    private static IObservable<Result<FileEntryData>> GetFileEntryData(Diff diff, IZafiroDirectory source)
    {
        return Observable.FromAsync(async () =>
        {
            var file = await source.GetFile(source.Path.Combine(diff.Path));
            var size = await file.Bind(zafiroFile => zafiroFile.Size());
            return file.Combine(size, (a, b)=> new FileEntryData(a, b));
        });
    }

    public IObservable<ISyncAction> OnRightOnly(Diff diff, IZafiroDirectory source)
    {
        var action = Observable
            .FromAsync(() => source.GetFile(source.Path.Combine(diff.Path)))
            .Select(result => result.Map(f => new DeleteAction(f)))
            .Successes();

        return action;
    }

    private static IObservable<ISyncAction> OnLeftOnly(Diff diff, IZafiroDirectory source, IZafiroDirectory destination)
    {
        var getSource = () => source.GetFile(source.Path.Combine(diff.Path));
        var getDestination = () => destination.GetFile(destination.Path.Combine(diff.Path));

        return getSource.Combine(getDestination, (o, d) => new CopyAction(o, d))
            .Successes();
    }
}

internal class FileEntryData
{
    public IZafiroFile ZafiroFile { get; }
    public long Size { get; }

    public FileEntryData(IZafiroFile zafiroFile, long size)
    {
        ZafiroFile = zafiroFile;
        Size = size;
    }
}