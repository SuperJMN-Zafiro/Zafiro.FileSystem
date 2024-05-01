using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Lightweight;
using Zafiro.Reactive;

namespace Zafiro.FileSystem;

public static class ByteProviderMixin
{
    public static IObservable<Result> DumpTo(this IByteProvider byteProvider, Stream stream)
    {
        return byteProvider.Bytes.DumpTo(stream);
    }
}