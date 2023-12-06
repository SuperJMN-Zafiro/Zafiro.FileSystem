using Zafiro.FileSystem;

namespace Zafiro.FileSystem2;

public interface IFileSystemRoot : IObservableFileSystem
{
    IZafiroFile2 GetFile(ZafiroPath path);
    IZafiroDirectory2 GetDirectory(ZafiroPath path);
}