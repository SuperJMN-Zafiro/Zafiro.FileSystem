using System.IO.Abstractions;
using System.Reactive.Linq;
using Zafiro.Reactive;

namespace Zafiro.FileSystem;

public class FileData : IData
{
    public FileData(IFileInfo file)
    {
        Bytes = Observable.Using(file.OpenRead, stream => stream.ToObservableChunked());
        Length = file.Length;
    }

    public long Length { get; }

    public IObservable<byte[]> Bytes { get; }
}