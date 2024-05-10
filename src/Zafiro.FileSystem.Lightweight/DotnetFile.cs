using System.IO.Abstractions;

namespace Zafiro.FileSystem.Lightweight;

internal class DotnetFile : IFile
{
    private readonly IFileInfo fileInfo;
    private readonly FileData data;

    public DotnetFile(IFileInfo fileInfo)
    {
        data = new FileData(fileInfo);
        this.fileInfo = fileInfo;
    }

    public string Name => fileInfo.Name;
    public IObservable<byte[]> Bytes => data.Bytes;
    public long Length => data.Length;

    public override string ToString() => fileInfo.ToString() ?? "";
}