using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Evolution;

public interface IFileSystem2
{
    Task<Result> CreateFile(ZafiroPath path);
    IObservable<byte> Contents(ZafiroPath path);
    Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes);
    Task<Result> CreateDirectory(ZafiroPath path);
    Task<Result<FileProperties>> GetFileProperties(ZafiroPath path);
    Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path);
    Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path);
    Task<Result<bool>> ExistDirectory(ZafiroPath path);
    Task<Result<bool>> ExistFile(ZafiroPath path);
    Task<Result> DeleteFile(ZafiroPath path);
    Task<Result> DeleteDirectory(ZafiroPath path);
}