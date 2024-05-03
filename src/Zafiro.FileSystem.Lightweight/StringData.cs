using System.Reactive.Linq;
using System.Text;
using Zafiro.Mixins;

namespace Zafiro.FileSystem.Lightweight;

public class StringData : IData
{
    public StringData(string content, Encoding encoding)
    {
        Bytes = content.ToBytes(encoding).ToObservable().Buffer(1024).Select(list => list.ToArray());
        Length = content.Length;
    }

    public IObservable<byte[]> Bytes { get; }
    public long Length { get; }
}