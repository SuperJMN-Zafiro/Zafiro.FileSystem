using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.IO;
using Zafiro.Mixins;

namespace Zafiro.FileSystem;

public static class ZafiroFileExtensions
{
    public static Task<Result> Copy(this IZafiroFile source, IZafiroFile destination, Maybe<IObserver<IProportionProgress>> progress, TimeSpan? readTimeout = default, CancellationToken cancellationToken = default)
    {
        return GetStream(source, readTimeout).Bind(sourceStream => CopyWithRetries(sourceStream, destination, progress, cancellationToken));
    }

    private static async Task<Result> CopyWithRetries(ObservableStream sourceStream, IZafiroFile destination, Maybe<IObserver<IProportionProgress>> progress, CancellationToken cancellationToken)
    {
        return await Observable
            .FromAsync(() => CopyStreamToFile(sourceStream, destination, progress, cancellationToken))
            .RetryWithBackoffStrategy();
    }

    private static async Task<Result> CopyStreamToFile(ObservableStream sourceStream, IZafiroFile destinationFile, Maybe<IObserver<IProportionProgress>> progress, CancellationToken cancellationToken)
    {
        var maybeSubscription = progress.Map(observer => sourceStream.Positions.Select(p => new ProportionProgress((double)p / sourceStream.Length)).Subscribe(observer));
        var result = await destinationFile.SetContents(sourceStream, cancellationToken);
        maybeSubscription.Execute(x => x.Dispose());
        return result;
    }

    private static Task<Result<ObservableStream>> GetStream(IZafiroFile zafiroFile, TimeSpan? readTimeout = default)
    {
        var streamResult = zafiroFile.GetContents();
        var timeoutAfter = readTimeout ?? TimeSpan.FromDays(1);

        return streamResult
            .Bind(stream => GetCompatibleStream(zafiroFile, stream, timeoutAfter));
    }

    private static async Task<Result<ObservableStream>> GetCompatibleStream(IZafiroFile zafiroFile, Stream stream, TimeSpan timeoutAfter)
    {
        var timingOutStream = new ReadTimeOutStream(stream){ ReadTimeout = (int) timeoutAfter.TotalMilliseconds};

        if (timingOutStream.CanSeek)
        {
            return new ObservableStream(timingOutStream);
        }

        var size = await zafiroFile.Size();
        return size.Map(l => new ObservableStream(new AlwaysForwardStream(timingOutStream, l)));
    }
}