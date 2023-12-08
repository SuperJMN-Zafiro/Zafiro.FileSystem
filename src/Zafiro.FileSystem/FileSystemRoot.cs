using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class FileSystemRoot : IFileSystemRoot
{
    private readonly IObservableFileSystem obsFs;

    public FileSystemRoot(IObservableFileSystem obsFs)
    {
        this.obsFs = obsFs;
    }

    public IZafiroFile GetFile(ZafiroPath path) => new ZafiroFile(path, this);
    public IZafiroDirectory GetDirectory(ZafiroPath path) => new ZafiroDirectory(path, this);
    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles(ZafiroPath path) => obsFs.GetFilePaths(path).Map(paths => paths.Select(zafiroPath => (IZafiroFile)new ZafiroFile(zafiroPath, this)));
    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories(ZafiroPath path) => obsFs.GetFilePaths(path).Map(paths => paths.Select(zafiroPath => (IZafiroDirectory)new ZafiroDirectory(zafiroPath, this)));
    public Task<Result<bool>> ExistFile(ZafiroPath path) => obsFs.ExistFile(path);
    public Task<Result> DeleteFile(ZafiroPath path) => obsFs.DeleteFile(path);
    public Task<Result> DeleteDirectory(ZafiroPath path) => obsFs.DeleteDirectory(path);
    public Task<Result<bool>> ExistDirectory(ZafiroPath path) => obsFs.ExistDirectory(path);
    public Task<Result> CreateFile(ZafiroPath path) => obsFs.CreateFile(path);
    public IObservable<byte> GetFileContents(ZafiroPath path) => obsFs.GetFileContents(path);
    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes) => obsFs.SetFileContents(path, bytes);
    public Task<Result> CreateDirectory(ZafiroPath path) => obsFs.CreateDirectory(path);
    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path) => obsFs.GetFileProperties(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path) => obsFs.GetFilePaths(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path) => obsFs.GetDirectoryPaths(path);
    public IObservable<FileSystemChange> Changed => obsFs.Changed;
}