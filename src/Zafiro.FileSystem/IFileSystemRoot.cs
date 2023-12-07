using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IFileSystemRoot : IObservableFileSystem
{
    IZafiroFile GetFile(ZafiroPath path);
    IZafiroDirectory GetDirectory(ZafiroPath path);
    Task<Result<IEnumerable<IZafiroFile>>> GetFiles(ZafiroPath path);
    Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories(ZafiroPath path);
}