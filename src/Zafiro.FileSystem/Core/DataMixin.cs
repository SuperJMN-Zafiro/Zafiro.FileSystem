using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Core;
using Zafiro.Mixins;
using Zafiro.Reactive;

namespace Zafiro.FileSystem.Core;

public static class DataMixin
{
    public static IObservable<Result> ChunkedDump(this IData data, Stream stream, CancellationToken cancellationToken = default)
    {
        return data.Bytes.DumpTo(stream, cancellationToken: cancellationToken);
    }

    public static async Task<Result> DumpTo(this IData data, Stream stream, CancellationToken cancellationToken = default)
    {
        var chuckResults = await data.ChunkedDump(stream, cancellationToken: cancellationToken).ToList();
        return chuckResults.Combine();
    }

    public static async Task<Result> DumpTo(this IData data, string path, CancellationToken cancellationToken = default)
    {
        using (var stream = File.Open(path, FileMode.Create))
        {
            return await data.DumpTo(stream, cancellationToken);
        }
    }

    public static byte[] Bytes(this IData data)
    {
        return data.Bytes.ToEnumerable().Flatten().ToArray();
    }
}