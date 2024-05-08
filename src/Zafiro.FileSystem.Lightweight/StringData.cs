using System.Reactive.Linq;
using System.Text;
using Zafiro.Mixins;

namespace Zafiro.FileSystem.Lightweight;

public class StringData : IData
{
    public StringData(string content) : this(content, Encoding.Default)
    {
    }
    
    public StringData(string content, Encoding encoding)
    {
        Content = content;
        Bytes = content.ToBytes(encoding).ToObservable().Buffer(1024).Select(list => list.ToArray());
        Length = content.Length;
    }

    public string Content { get; }

    public IObservable<byte[]> Bytes { get; }
    public long Length { get; }
    
    public static implicit operator StringData(string content)
    {
        return new StringData(content, Encoding.Default);
    }
}