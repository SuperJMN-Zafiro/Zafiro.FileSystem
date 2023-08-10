using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.IO;
using Zafiro.Mixins;
using Zafiro.ProgressReporting;

namespace Zafiro.FileSystem;

public static class ZafiroFileExtensions
{
    public static Task<Result> Copy(this IZafiroFile source, IZafiroFile destination, Maybe<IObserver<RelativeProgress<long>>> progress, TimeSpan? readTimeout = default)
    {
        return GetStream(source, readTimeout).Bind(sourceStream => CopyWithRetries(sourceStream, destination, progress));
    }

    private static async Task<Result> CopyWithRetries(ObservableStream sourceStream, IZafiroFile destination, Maybe<IObserver<RelativeProgress<long>>> progress)
    {
        return await Observable
            .FromAsync(() => CopyStreamToFile(sourceStream, destination, progress))
            .RetryWithBackoffStrategy();
    }

    private static async Task<Result> CopyStreamToFile(ObservableStream sourceStream, IZafiroFile destinationFile, Maybe<IObserver<RelativeProgress<long>>> progress)
    {
        var maybeSubscription = progress.Map(observer => sourceStream.Positions.Select(l => new RelativeProgress<long>(sourceStream.Length, l)).Subscribe(observer));
        var result = await destinationFile.SetContents(sourceStream);
        maybeSubscription.Execute(x => x.Dispose());
        return result;
    }

    private static Task<Result<ObservableStream>> GetStream(IZafiroFile zafiroFile, TimeSpan? readTimeout = default)
    {
        var streamResult = zafiroFile.GetContents();
        var timeoutAfter = readTimeout ?? TimeSpan.FromDays(1);

        return streamResult
            .Map(stream => new ReadTimeOutStream(stream) { ReadTimeout = (int)timeoutAfter.TotalMilliseconds })
            .Bind(stream => GetCompatibleStream(zafiroFile, stream));
    }

    private static async Task<Result<ObservableStream>> GetCompatibleStream(IZafiroFile zafiroFile, Stream stream)
    {
        var timingOutStream = new ReadTimeOutStream(stream);

        if (timingOutStream.CanSeek)
        {
            return new ObservableStream(timingOutStream);
        }

        var size = await zafiroFile.Size();
        return size.Map(l => new ObservableStream(new AlwaysForwardStream(timingOutStream, l)));
    }
}