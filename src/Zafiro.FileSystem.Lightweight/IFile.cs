namespace Zafiro.FileSystem.Lightweight;

public interface IFile : IStreamOpen
{
    public string Name { get; }
}