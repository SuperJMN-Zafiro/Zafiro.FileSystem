using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.FileSystem2;

public interface IFileSystem2
{
    public Task<Result> CreateFile(ZafiroPath path);
    public IObservable<byte> Contents(ZafiroPath path);
    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes);
    public Task<Result> CreateFolder(ZafiroPath path);
    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path);
    Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path);
    Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path);
    Task<Result<bool>> ExistDirectory(ZafiroPath path);
}