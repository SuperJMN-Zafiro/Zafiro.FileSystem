using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Mixins;
using Zafiro.Reactive;

namespace Zafiro.FileSystem;

public static class ByteProviderMixin
{
    public static IObservable<Result> ChunkedDump(this IObservableDataStream observableDataStream, Stream stream)
    {
        return observableDataStream.Bytes.DumpTo(stream);
    }
    
    public static async Task<Result> DumpTo(this IObservableDataStream observableDataStream, Stream stream)
    {
        var chuckResults = await observableDataStream.ChunkedDump(stream).ToList();
        return chuckResults.Combine();
    }
    
    public static byte[] Bytes(this IObservableDataStream observableDataStream)
    {
        return observableDataStream.Bytes.ToEnumerable().Flatten().ToArray();
    }
}