namespace Zafiro.FileSystem;

public interface IFileSystem
{
    Task<IZafiroDirectory> GetDirectory(ZafiroPath path);
    Task<IZafiroFile> GetFile(ZafiroPath path);
}