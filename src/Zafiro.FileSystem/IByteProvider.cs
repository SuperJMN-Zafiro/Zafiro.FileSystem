namespace Zafiro.FileSystem.Lightweight;

public interface IByteProvider
{
    public IObservable<byte[]> Bytes { get; }
    public long Length { get; }
}