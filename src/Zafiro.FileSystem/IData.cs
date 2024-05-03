namespace Zafiro.FileSystem;

public interface IData
{
    public IObservable<byte[]> Bytes { get; }
    public long Length { get; }
}