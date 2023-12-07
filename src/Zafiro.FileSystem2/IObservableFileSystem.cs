namespace Zafiro.FileSystem2;

public interface IObservableFileSystem : IFileSystem2
{
    IObservable<FileSystemChange> Changed { get; }
}