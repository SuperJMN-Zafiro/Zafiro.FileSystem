using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.ProgressReporting;

namespace Zafiro.FileSystem;

[PublicAPI]
public class Syncer
{
    public Maybe<ILogger> Logger { get; }
    public bool SkipIdentical { get; set; }

    public Syncer(Maybe<ILogger> logger)
    {
        Logger = logger;
    }

    public IObservable<ISyncAction> Sync(IZafiroDirectory source, IZafiroDirectory destination, IEnumerable<Diff> diffs)
    {
        return SyncItems(source, destination, diffs);
    }

    public IObservable<ISyncAction> OnRightOnly(Diff diff, IZafiroDirectory source)
    {
        return Observable
            .FromAsync(() => source.GetFile(source.Path.Combine(diff.Path)))
            .Select(result => result.Map(f => new DeleteAction(f)))
            .Successes();
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
            FileDiffStatus.LeftOnly => GetFileEntries(source, destination, diff.Path)
                .Successes()
                .SelectMany(GetCopyAction),
            FileDiffStatus.Both => GetFileEntries(source, destination, diff.Path)
                .Successes()
                .SelectMany(GetCopyAction),
            FileDiffStatus.Invalid => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException(nameof(diff.Status), "The diff status is not valid")
        };
    }

    private IObservable<Result<IZafiroFile>> GetFileEntry(IZafiroDirectory directory, ZafiroPath path)
    {
        return GetEntry(directory, path);
    }

    private IObservable<Result<(IZafiroFile, IZafiroFile)>> GetFileEntries(IZafiroDirectory directory, IZafiroDirectory destination, ZafiroPath path)
    {
        return ReactiveResultMixin
            .SelectMany(GetFileEntry(directory, path),
                _ => GetFileEntry(destination, path),
                (l, r) => (l, r));
    }

    private IObservable<ISyncAction> GetCopyAction((IZafiroFile, IZafiroFile) copyData)
    {
        return Observable.FromAsync(async () => await GetFinalCopyAction(copyData.Item1, copyData.Item2)).Successes();
    }

    private async Task<Result<ISyncAction>> GetFinalCopyAction(IZafiroFile source, IZafiroFile destination)
    {
        var r = await AreEqual(source, destination);
        var result = r.Map<bool, ISyncAction>(b =>
        {
            if (SkipIdentical && b)
            {
                return new SkipFileAction(source, destination);
            }

            return new CopyAction(source, destination);
        });
        return result;
    }

    private Task<Result<bool>> AreEqual(IZafiroFile left, IZafiroFile right)
    {
        var leftSize = GetSize(left);
        var rightSize = GetSize(right);
        
        return leftSize.CombineAndMap(rightSize, (sizeLeft, sizeRight) => sizeLeft != -1 && sizeRight != -1 && sizeLeft == sizeRight);
    }

    private Task<Result<long>> GetSize(IZafiroFile file)
    {
        return Async.Await(file.Size)
            .TapError(msg => Logger.Execute(l => l.Warning(msg)))
            .Compensate(_ => Result.Success<long>(-1));
    }

    private IObservable<Result<IZafiroFile>> GetEntry(IZafiroDirectory directory, ZafiroPath path)
    {
        return Observable.FromAsync(() => directory.GetFile(directory.Path.Combine(path)));
    }
}

public class SkipFileAction : ISyncAction
{
    public SkipFileAction(IZafiroFile source, IZafiroFile destination)
    {
        Source = source;
        Destination = destination;
    }

    public IZafiroFile Source { get; }
    public IZafiroFile Destination { get; }

    public IObservable<RelativeProgress<long>> Progress => Observable.Return(new RelativeProgress<long>(1, 1));

    public Task<Result> Sync()
    {
        return Task.FromResult(Result.Success());
    }
}