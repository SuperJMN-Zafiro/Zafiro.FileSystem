using System.IO.Abstractions;

namespace Zafiro.FileSystem.Lightweight;

public class SystemIOFile: IFile
{
    private readonly IFileInfo file;
    private readonly FileObservableDataStream observableDataStream;

    public SystemIOFile(IFileInfo file)
    {
        this.file = file;
        observableDataStream = new FileObservableDataStream(file);
    }

    public string Name => file.Name;
    public IObservable<byte[]> Bytes => observableDataStream.Bytes;
    public long Length => observableDataStream.Length;
}