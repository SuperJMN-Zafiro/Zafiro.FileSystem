using System.IO.Abstractions;
using Zafiro.DataModel;

namespace Zafiro.FileSystem.Local;

public class DotNetFile : IFile
{
    private readonly IFileInfo fileInfo;
    private readonly FileInfoData infoData;

    public DotNetFile(IFileInfo fileInfo)
    {
        infoData = new FileInfoData(fileInfo);
        this.fileInfo = fileInfo;
    }

    public string Name => fileInfo.Name;
    public IObservable<byte[]> Bytes => infoData.Bytes;
    public long Length => infoData.Length;

    public override string ToString() => fileInfo.ToString() ?? "";
}