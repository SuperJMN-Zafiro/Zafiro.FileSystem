using System.IO.Abstractions;
using Zafiro.DataModel;

namespace Zafiro.FileSystem.Local;

public class DotNetFile : IFile
{
    private readonly IFileInfo fileInfo;
    public FileInfoData FileInfoData { get; }

    public DotNetFile(IFileInfo fileInfo)
    {
        FileInfo = fileInfo;
        FileInfoData = new FileInfoData(fileInfo);
        this.fileInfo = fileInfo;
    }

    public IFileInfo FileInfo { get; }

    public string Name => fileInfo.Name;
    public IObservable<byte[]> Bytes => FileInfoData.Bytes;
    public long Length => FileInfoData.Length;

    public override string ToString() => fileInfo.ToString() ?? "";
}