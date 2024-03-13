using System.Reactive.Concurrency;
using CSharpFunctionalExtensions;
using System.Reactive.Linq;
using Zafiro.Actions;

namespace Zafiro.FileSystem;

public static class ZafiroFileExtensions
{
    public static Task<Result> Copy(this IZafiroFile source, IZafiroFile destination, Maybe<IObserver<LongProgress>> progress, IScheduler? progressScheduler = default, IScheduler? timeoutScheduler = default, TimeSpan? readTimeout = default, CancellationToken cancellationToken = default)
    {
        return source.GetData()
            .Map(s => new PositionReportingStream(s))
            .Bind(async stream =>
            {
                var subscription = progress.Map(p => stream.Positions.Select(x => new LongProgress(x, stream.Length)).Subscribe(p));
                var result = await destination.SetData(stream, cancellationToken);
                subscription.Execute(d => d.Dispose());
                return result;
            });
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