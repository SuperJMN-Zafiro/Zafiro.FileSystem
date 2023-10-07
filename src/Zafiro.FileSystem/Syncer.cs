using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

[PublicAPI]
public class Syncer
{
    public Syncer(Maybe<ILogger> logger)
    {
        Logger = logger;
    }

    public Maybe<ILogger> Logger { get; }
    public bool SkipIdentical { get; set; }
    public bool DeleteNonExistent { get; set; }
    public bool CanOverwrite { get; set; }

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
            FileDiffStatus.RightOnly => RighOnly(diff, source),
            FileDiffStatus.LeftOnly => GetFileEntries(source, destination, diff.Path)
                .Successes()
                .SelectMany(LeftOnly),
            FileDiffStatus.Both => GetFileEntries(source, destination, diff.Path)
                .Successes()
                .SelectMany(Both),
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

    private IObservable<ISyncAction> LeftOnly((IZafiroFile, IZafiroFile) copyData)
    {
        return Observable.Return(new CopyAction(copyData.Item1, copyData.Item2));
    }

    private IObservable<ISyncAction> RighOnly(Diff diff, IZafiroDirectory source)
    {
        return Observable
            .FromAsync(() => source.GetFile(source.Path.Combine(diff.Path)))
            .Select(result => result
                .Map(f => DeleteNonExistent ? (ISyncAction) new DeleteAction(f) : new SkipFileAction(f, Maybe<IZafiroFile>.None)))
            .Successes();
    }

    private IObservable<ISyncAction> Both((IZafiroFile, IZafiroFile) copyData)
    {
        return Observable.FromAsync(async () => await GetFinalCopyAction(copyData.Item1, copyData.Item2).ConfigureAwait(false)).Successes();
    }

    private async Task<Result<ISyncAction>> GetFinalCopyAction(IZafiroFile source, IZafiroFile destination)
    {
        if (!CanOverwrite)
        {
            return new SkipFileAction(source, Maybe.From(destination));
        }

        var r = await AreEqual(source, destination).ConfigureAwait(false);
        var result = r.Map<bool, ISyncAction>(areEqual =>
        {
            if (SkipIdentical && areEqual)
            {
                return new SkipFileAction(source, Maybe<IZafiroFile>.From(destination));
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
        return Observable.FromAsync(() => directory.FileSystem.GetFile(directory.Path.Combine(path)));
    }
}