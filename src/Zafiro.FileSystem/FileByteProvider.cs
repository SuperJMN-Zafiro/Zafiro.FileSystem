using System.IO.Abstractions;
using System.Reactive.Linq;
using Zafiro.FileSystem.Lightweight;
using Zafiro.Reactive;

namespace Zafiro.FileSystem;

public class FileByteProvider : IByteProvider
{
    public FileByteProvider(IFileInfo file)
    {
        Bytes = Observable.Using(file.OpenRead, stream => stream.ToObservableChunked());
        Length = file.Length;
    }

    public long Length { get; }

    public IObservable<byte[]> Bytes { get; }
}