
using System.IO.Abstractions;
using Zafiro.DataModel;

namespace Zafiro.FileSystem;

public class SystemIOFile: IFile
{
    private readonly IFileInfo file;
    private readonly FileInfoData infoData;

    public SystemIOFile(IFileInfo file)
    {
        this.file = file;
        infoData = new FileInfoData(file);
    }

    public string Name => file.Name;
    public IObservable<byte[]> Bytes => infoData.Bytes;
    public long Length => infoData.Length;
}