using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.MoveToZafiro;

public static class ReactiveStreamMixin
{
    public static IObservable<Result> DumpTo(this IObservable<byte[]> source, Stream output, TimeSpan? chunkReadTimeout = default, IScheduler? scheduler = default, int bufferSize = 4096)
    {
        scheduler ??= Scheduler.Default;
        chunkReadTimeout ??= TimeSpan.FromDays(1);

        return source
            .Select(chunk =>
            {
                return Observable
                    .FromAsync(ct => Result.Try(() => output.WriteAsync(chunk.ToArray(), 0, chunk.Length, ct)), scheduler)
                    .Timeout(chunkReadTimeout.Value, scheduler);
            })
            .Concat();
    }
}