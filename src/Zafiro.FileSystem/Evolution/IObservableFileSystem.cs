namespace Zafiro.FileSystem.Evolution;

public interface IObservableFileSystem : IFileSystem2
{
    IObservable<FileSystemChange> Changed { get; }
}