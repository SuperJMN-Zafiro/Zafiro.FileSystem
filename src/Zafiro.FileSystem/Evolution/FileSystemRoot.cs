using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Evolution;

public class FileSystemRoot : IFileSystemRoot
{
    private readonly IObservableFileSystem obsFs;

    public FileSystemRoot(IObservableFileSystem obsFs)
    {
        this.obsFs = obsFs;
    }

    public IZafiroFile2 GetFile(ZafiroPath path) => new ZafiroFile2(path, this);
    public IZafiroDirectory2 GetDirectory(ZafiroPath path) => new ZafiroDirectory2(path, this);
    public Task<Result<IEnumerable<IZafiroFile2>>> GetFiles(ZafiroPath path) => obsFs.GetFilePaths(path).Map(paths => paths.Select(zafiroPath => (IZafiroFile2)new ZafiroFile2(zafiroPath, this)));
    public Task<Result<IEnumerable<IZafiroDirectory2>>> GetDirectories(ZafiroPath path) => obsFs.GetFilePaths(path).Map(paths => paths.Select(zafiroPath => (IZafiroDirectory2)new ZafiroDirectory2(zafiroPath, this)));
    public Task<Result<bool>> ExistFile(ZafiroPath path) => obsFs.ExistFile(path);
    public Task<Result> DeleteFile(ZafiroPath path) => obsFs.DeleteFile(path);
    public Task<Result> DeleteDirectory(ZafiroPath path) => obsFs.DeleteDirectory(path);
    public Task<Result<bool>> ExistDirectory(ZafiroPath path) => obsFs.ExistDirectory(path);
    public Task<Result> CreateFile(ZafiroPath path) => obsFs.CreateFile(path);
    public IObservable<byte> Contents(ZafiroPath path) => obsFs.Contents(path);
    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes) => obsFs.SetFileContents(path, bytes);
    public Task<Result> CreateDirectory(ZafiroPath path) => obsFs.CreateDirectory(path);
    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path) => obsFs.GetFileProperties(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path) => obsFs.GetFilePaths(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path) => obsFs.GetDirectoryPaths(path);
    public IObservable<FileSystemChange> Changed => obsFs.Changed;
}