using System.IO.Abstractions;

namespace Zafiro.FileSystem.Lightweight;

public class SystemIOFile: IFile
{
    private readonly IFileInfo file;
    private readonly FileByteProvider byteProvider;

    public SystemIOFile(IFileInfo file)
    {
        this.file = file;
        byteProvider = new FileByteProvider(file);
    }

    public string Name => file.Name;
    public IObservable<byte[]> Bytes => byteProvider.Bytes;
    public long Length => byteProvider.Length;
}