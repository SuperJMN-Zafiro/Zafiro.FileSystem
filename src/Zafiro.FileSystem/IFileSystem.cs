using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IFileSystem
{
    Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path);
    Task<Result<IZafiroFile>> GetFile(ZafiroPath path);
    Task<Result<ZafiroPath>> GetRoot();
}