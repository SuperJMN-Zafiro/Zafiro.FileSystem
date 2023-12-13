using System.Reactive.Disposables;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public static class ZafiroFileExtensions2
{
    public static async Task<Result> Copy(this IZafiroFile source, IZafiroFile destination, Maybe<IObserver<LongProgress>> progress, TimeSpan? readTimeout = default, CancellationToken cancellationToken = default)
    {
    var asMaybe = source.Properties.Map(f => f.Length).AsMaybe();
        using (await InjectProgress(asMaybe, source.Contents, progress))
        {
            var contents = await destination.SetContents(source.Contents).ConfigureAwait(false);
            return contents;
        }
    }

    public static async Task<IDisposable> InjectProgress(Task<Maybe<long>> length, IObservable<byte> bytes, Maybe<IObserver<LongProgress>> progress)
    {
        var taggedBytes = bytes.Select((b, i) => (b, i: i + 1));

        var lengthResult = await length.ConfigureAwait(false);
        var maybeDisposable = lengthResult.Bind(l => progress.Map(f => taggedBytes.Buffer(TimeSpan.FromSeconds(1))
            .Where(list => list.Any())
            .Select(tuple => new LongProgress(tuple.Last().i, l)).Subscribe(f)));

        return maybeDisposable.GetValueOrDefault(Disposable.Empty);
    }
}