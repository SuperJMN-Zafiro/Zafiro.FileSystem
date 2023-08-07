using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.IO;
using Zafiro.ProgressReporting;

namespace Zafiro.FileSystem;

public static class ZafiroFileExtensions
{
    public static Task<Result> Copy(this IZafiroFile source, IZafiroFile destination, Maybe<IObserver<RelativeProgress<long>>> progress, TimeSpan? readTimeout = default)
    {
        return GetStream(source)
            .Bind(stream => CopyToStream(destination, stream, progress));
    }

    private static async Task<Result> CopyToStream(IZafiroFile destination, ObservableStream stream, Maybe<IObserver<RelativeProgress<long>>> progress)
    {
        var maybeSubscription = progress.Map(observer => stream.Positions.Select(l => new RelativeProgress<long>(stream.Length, l)).Subscribe(observer));
        var result = await destination.SetContents(stream);
        maybeSubscription.Execute(x => x.Dispose());
        return result;
    }

    private static Task<Result<ObservableStream>> GetStream(IZafiroFile zafiroFile)
    {
        var streamResult = zafiroFile.GetContents();

        return streamResult.Bind(async stream =>
        {
            if (stream.CanSeek)
            {
                return new ObservableStream(stream);
            }

            var size = await zafiroFile.Size();
            return size.Map(l => new ObservableStream(new AlwaysForwardStream(stream, l)));
        });
    }
}