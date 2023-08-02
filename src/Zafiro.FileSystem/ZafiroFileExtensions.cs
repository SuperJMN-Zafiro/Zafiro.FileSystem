using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.IO;

namespace Zafiro.FileSystem;

public static class ZafiroFileExtensions
{
    public static async Task<Result> Copy(this IZafiroFile source, IZafiroFile destination, Maybe<IObserver<double>> progress, TimeSpan? readTimeout = default)
    {
        var contentsResult = await source.GetContents();
        var setContentsResult = await contentsResult.Bind(async stream =>
        {
            using (stream)
            {
                var obsStream = new ObservableStream(new ReadTimeOutStream(stream));

                if (readTimeout.HasValue && obsStream.CanTimeout)
                {
                    obsStream.ReadTimeout = (int)readTimeout.Value.TotalMilliseconds;
                }

                var maybeSubscription = progress.Map(observer => obsStream.Positions.Select(l => (double)l / stream.Length).Subscribe(observer));
                var contents = await destination.SetContents(obsStream);
                maybeSubscription.Execute(x => x.Dispose());
                return contents;
            }
        });
        return setContentsResult;
    }
}