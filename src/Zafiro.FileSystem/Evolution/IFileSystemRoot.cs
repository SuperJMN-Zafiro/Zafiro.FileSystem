using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Evolution;

public interface IFileSystemRoot : IObservableFileSystem
{
    IZafiroFile2 GetFile(ZafiroPath path);
    IZafiroDirectory2 GetDirectory(ZafiroPath path);
    Task<Result<IEnumerable<IZafiroFile2>>> GetFiles(ZafiroPath path);
    Task<Result<IEnumerable<IZafiroDirectory2>>> GetDirectories(ZafiroPath path);
}