using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.FileSystem.Evolution;
using Zafiro.IO;
using Zafiro.Mixins;

namespace Zafiro.FileSystem;

public static class ZafiroFileExtensions2
{
    public static Task<Result> Copy(this IZafiroFile2 source, IZafiroFile2 destination, Maybe<IObserver<LongProgress>> progress, TimeSpan? readTimeout = default, CancellationToken cancellationToken = default)
    {
        return destination
            .SetContents(source.Contents);
        //return GetStream(source, readTimeout).Bind(async sourceStream =>
        //{
        //    var copyWithRetries = await CopyWithRetries(sourceStream, destination, progress, cancellationToken).ConfigureAwait(false);
        //    await sourceStream.DisposeAsync().ConfigureAwait(false);
        //    return copyWithRetries;
        //});
    }

    //private static async Task<Result> CopyWithRetries(ObservableStream sourceStream, IZafiroFile destination, Maybe<IObserver<LongProgress>> progress, CancellationToken cancellationToken)
    //{
    //    return await Observable
    //        .FromAsync(() => CopyStreamToFile(sourceStream, destination, progress, cancellationToken))
    //        .RetryWithBackoffStrategy();
    //}

    //private static async Task<Result> CopyStreamToFile(ObservableStream sourceStream, IZafiroFile destinationFile, Maybe<IObserver<LongProgress>> progress, CancellationToken cancellationToken)
    //{
    //    var maybeSubscription = progress.Map(observer => GetProgressObservable(sourceStream).Subscribe(observer));
    //    var result = await destinationFile.SetContents(sourceStream, cancellationToken).ConfigureAwait(false);
    //    maybeSubscription.Execute(x => x.Dispose());
    //    return result;
    //}

    //private static IObservable<LongProgress> GetProgressObservable(ObservableStream sourceStream)
    //{
    //    return sourceStream.Positions
    //        .Select(processed => new LongProgress(processed, sourceStream.Length))
    //        .StartWith(new LongProgress(0, sourceStream.Length));
    //}

    //private static Task<Result<ObservableStream>> GetStream(IZafiroFile zafiroFile, TimeSpan? readTimeout = default)
    //{
    //    var streamResult = zafiroFile.GetContents();
    //    var timeoutAfter = readTimeout ?? TimeSpan.FromDays(1);

    //    return streamResult
    //        .Bind(stream => GetCompatibleStream(zafiroFile, stream, timeoutAfter));
    //}

    //private static async Task<Result<ObservableStream>> GetCompatibleStream(IZafiroFile zafiroFile, Stream stream, TimeSpan timeoutAfter)
    //{
    //    var timingOutStream = new ReadTimeOutStream(stream){ ReadTimeout = (int) timeoutAfter.TotalMilliseconds};

    //    if (timingOutStream.CanSeek)
    //    {
    //        return new ObservableStream(timingOutStream);
    //    }

    //    var size = await zafiroFile.Size().ConfigureAwait(false);
    //    return size.Map(l => new ObservableStream(new AlwaysForwardStream(timingOutStream, l)));
    //}

    public static Task<Result<IZafiroFile>> Translate(this IZafiroFile target, IZafiroDirectory sourceRoot, IZafiroDirectory destination)
    {
        var subPath = target.Path.MakeRelativeTo(sourceRoot.Path);
        return destination.FileSystem.GetFile(destination.Path.Combine(subPath));
    }
}