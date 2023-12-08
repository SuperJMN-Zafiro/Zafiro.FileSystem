namespace Zafiro.FileSystem;

public interface IObservableFileSystem : IZafiroFileSystem
{
    IObservable<FileSystemChange> Changed { get; }
}