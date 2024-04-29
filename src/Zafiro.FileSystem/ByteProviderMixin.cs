using CSharpFunctionalExtensions;
using Zafiro.Reactive;

namespace Zafiro.FileSystem.Lightweight;

public static class ByteProviderMixin
{
    public static IObservable<Result> DumpTo(this IByteProvider byteProvider, Stream stream)
    {
        return byteProvider.Bytes.DumpTo(stream);
    }
}