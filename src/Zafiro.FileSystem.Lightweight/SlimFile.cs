namespace Zafiro.FileSystem.Lightweight;

public class SlimFile : IFile
{
    public SlimFile(string name, IData data)
    {
        Name = name;
        Data = data;
    }

    public string Name { get; }
    public IData Data { get; }
    public IObservable<byte[]> Bytes => Data.Bytes;
    public long Length => Data.Length;
}