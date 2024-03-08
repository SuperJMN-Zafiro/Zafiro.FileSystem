using System.Reactive.Concurrency;
using CSharpFunctionalExtensions;
using System.Reactive.Linq;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Reactive;

namespace Zafiro.FileSystem;

public static class ZafiroFileExtensions
{
    public static async Task<Result> Copy(this IZafiroFile source, IZafiroFile destination, Maybe<IObserver<LongProgress>> progress, IScheduler? progressScheduler = default, IScheduler? timeoutScheduler = default, TimeSpan? readTimeout = default, CancellationToken cancellationToken = default)
    {
        var maybeLength = source.Properties.Map(f => f.Length).AsMaybe();
        var contents = await maybeLength
            .Map(l => source.Contents.ProgressDo(longProgress => progress.Execute(observer => observer.OnNext(longProgress)), l, TimeSpan.FromSeconds(1), progressScheduler ?? Scheduler.Default))
            .GetValueOrDefault(() => source.Contents)
            .ConfigureAwait(false);

        var contentsWithTimeout = contents.Timeout(readTimeout ?? TimeSpan.FromMinutes(1), scheduler: timeoutScheduler ?? Scheduler.Default);
        var result = await destination.SetContents(contentsWithTimeout, cancellationToken).ConfigureAwait(false);
        result.Tap(() => progress.Execute(observer => observer.OnCompleted()));
        result.TapError(() => progress.Execute(observer => observer.OnCompleted()));
        return result;
    }

    public static IZafiroFile Mirror(this IZafiroFile file, ZafiroPath root, IZafiroDirectory destinationRoot)
    {
        var relativeToRoot = file.Path.MakeRelativeTo(root);
        var translatedPath = destinationRoot.Path.Combine(relativeToRoot);
        var equivalentIn = destinationRoot.FileSystem.GetFile(translatedPath);
        return equivalentIn;
    }

    public static IZafiroDirectory Parent(this IZafiroFile file)
    {
        return file.FileSystem.GetDirectory(file.Path.Parent());
    }

    public static Task<Result<bool>> AreEqual(this IZafiroFile one, IZafiroFile two, IFileCompareStrategy strategy)
    {
        return strategy.Compare(one, two);
    }
}