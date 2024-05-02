using System.Text;

namespace Zafiro.FileSystem.Lightweight;

public class File : IFile
{
    private readonly StringObservableDataStream observableDataStream;

    public File(string name, string content)
    {
        observableDataStream = new StringObservableDataStream(content, Encoding.UTF8);
        Name = name;
    }

    public string Name { get; }

    public IObservable<byte[]> Bytes => observableDataStream.Bytes;
    public long Length => observableDataStream.Length;
}