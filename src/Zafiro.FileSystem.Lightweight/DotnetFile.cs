using System.IO.Abstractions;
using Zafiro.DataModel;

namespace Zafiro.FileSystem.Lightweight;

internal class DotnetFile : IFile
{
    private readonly IFileInfo fileInfo;
    private readonly FileInfoData infoData;

    public DotnetFile(IFileInfo fileInfo)
    {
        infoData = new FileInfoData(fileInfo);
        this.fileInfo = fileInfo;
    }

    public string Name => fileInfo.Name;
    public IObservable<byte[]> Bytes => infoData.Bytes;
    public long Length => infoData.Length;

    public override string ToString() => fileInfo.ToString() ?? "";
}