using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Mixins;

namespace Zafiro.FileSystem;

public static class ZafiroFileExtensions
{
    public static async Task<Result> Copy(this IZafiroFile source, IZafiroFile destination, Maybe<IObserver<LongProgress>> progress, TimeSpan? readTimeout = default, CancellationToken cancellationToken = default)
    {
        var maybeLength = source.Properties.Map(f => f.Length).AsMaybe();
        var contents = await maybeLength
            .Map(l => source.Contents.ProgressDo(l, longProgress => progress.Execute(observer => observer.OnNext(longProgress))))
            .GetValueOrDefault(() => source.Contents)
            .ConfigureAwait(false);

        var result = await destination.SetContents(contents).ConfigureAwait(false);
        result.Tap(() => progress.Execute(observer => observer.OnCompleted()));
        result.TapError(() => progress.Execute(observer => observer.OnCompleted()));
        return result;
    }

    public static IZafiroFile EquivalentIn(this IZafiroFile file, IZafiroDirectory destination)
    {
        return destination.FileSystem.GetFile(destination.Path.Combine(file.Path.Name()));
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