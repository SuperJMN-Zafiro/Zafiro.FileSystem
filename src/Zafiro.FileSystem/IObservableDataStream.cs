namespace Zafiro.FileSystem;

public interface IObservableDataStream
{
    public IObservable<byte[]> Bytes { get; }
    public long Length { get; }
}