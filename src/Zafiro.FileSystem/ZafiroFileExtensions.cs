using System.Reactive.Disposables;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public static class ZafiroFileExtensions
{
    public static async Task<Result> Copy(this IZafiroFile source, IZafiroFile destination, Maybe<IObserver<LongProgress>> progress, TimeSpan? readTimeout = default, CancellationToken cancellationToken = default)
    {
        var contents = source.Contents.Publish();
        var asMaybe = source.Properties.Map(f => f.Length).AsMaybe();

        using (contents.Connect())
        using (await InjectProgress(asMaybe, contents, progress))
        {
            var result = await destination.SetContents(contents).ConfigureAwait(false);
            return result;
        }
    }

    public static IZafiroFile EquivaletIn(this IZafiroFile file, IZafiroDirectory destination)
    {
        return destination.FileSystem.GetFile(destination.Path.Combine(file.Path.Name()));
    }

    public static IZafiroDirectory Parent(this IZafiroFile file)
    {
        return file.FileSystem.GetDirectory(file.Path.Parent());
    }

    private static async Task<IDisposable> InjectProgress(Task<Maybe<long>> length, IObservable<byte> bytes, Maybe<IObserver<LongProgress>> progress)
    {
        var taggedBytes = bytes.Select((b, i) => (b, i: i + 1));

        var lengthResult = await length.ConfigureAwait(false);
        var maybeDisposable = lengthResult.Bind(l => progress.Map(f => taggedBytes.Buffer(TimeSpan.FromSeconds(1))
            .Where(list => list.Any())
            .Select(tuple => new LongProgress(tuple.Last().i, l)).Subscribe(f)));

        return maybeDisposable.GetValueOrDefault(Disposable.Empty);
    }
}