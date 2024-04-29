using System.Text;

namespace Zafiro.FileSystem.Lightweight;

public class File : IFile
{
    private readonly StringByteProvider byteProvider;

    public File(string name, string content)
    {
        byteProvider = new StringByteProvider(content, Encoding.UTF8);
        Name = name;
    }

    public string Name { get; }

    public IObservable<byte[]> Bytes => byteProvider.Bytes;
    public long Length => byteProvider.Length;
}