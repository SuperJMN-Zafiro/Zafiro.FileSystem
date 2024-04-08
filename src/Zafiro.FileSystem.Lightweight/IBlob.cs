namespace Zafiro.FileSystem.Lightweight;

public interface IBlob : IGetStream
{
    public string Name { get; }
}