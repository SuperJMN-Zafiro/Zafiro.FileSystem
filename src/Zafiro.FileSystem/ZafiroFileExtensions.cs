using CSharpFunctionalExtensions;
using Zafiro.Core.IO;

namespace Zafiro.FileSystem;

public static class ZafiroFileExtensions
{
    public static async Task<Result> Copy(this IZafiroFile source, IZafiroFile destination, Maybe<IObserver<double>> progress)
    {
        var contentsResult = await source.GetContents();
        var setContentsResult = await contentsResult.Bind(async stream =>
        {
            var progressNotifyingStream = new ProgressNotifyingStream(stream);
            var maybeSubscription = progress.Map(observer => progressNotifyingStream.Progress.Subscribe(observer));
            var contents = await destination.SetContents(progressNotifyingStream);
            maybeSubscription.Execute(x => x.Dispose());
            return contents;
        });
        return setContentsResult;
    }
}