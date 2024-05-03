using System.Text;

namespace Zafiro.FileSystem.Lightweight;

public class File : IFile
{
    private readonly StringData data;

    public File(string name, string content)
    {
        data = new StringData(content, Encoding.UTF8);
        Name = name;
    }

    public string Name { get; }

    public IObservable<byte[]> Bytes => data.Bytes;
    public long Length => data.Length;
}