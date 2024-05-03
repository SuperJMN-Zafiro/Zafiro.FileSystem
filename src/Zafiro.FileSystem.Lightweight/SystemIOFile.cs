using System.IO.Abstractions;

namespace Zafiro.FileSystem.Lightweight;

public class SystemIOFile: IFile
{
    private readonly IFileInfo file;
    private readonly FileData data;

    public SystemIOFile(IFileInfo file)
    {
        this.file = file;
        data = new FileData(file);
    }

    public string Name => file.Name;
    public IObservable<byte[]> Bytes => data.Bytes;
    public long Length => data.Length;
}