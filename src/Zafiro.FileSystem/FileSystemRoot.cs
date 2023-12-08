using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class FileSystemRoot : IFileSystemRoot
{
    private readonly IObservableFileSystem fs;

    public FileSystemRoot(IObservableFileSystem fs)
    {
        this.fs = fs;
    }

    public IZafiroFile GetFile(ZafiroPath path) => new ZafiroFile(path, this);
    public IZafiroDirectory GetDirectory(ZafiroPath path) => new ZafiroDirectory(path, this);
    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles(ZafiroPath path) => fs.GetFilePaths(path).Map(paths => paths.Select(zafiroPath => (IZafiroFile) new ZafiroFile(zafiroPath, this)));
    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories(ZafiroPath path) => fs.GetDirectoryPaths(path).Map(paths => paths.Select(zafiroPath => (IZafiroDirectory) new ZafiroDirectory(zafiroPath, this)));
    public Task<Result<bool>> ExistFile(ZafiroPath path) => fs.ExistFile(path);
    public Task<Result> DeleteFile(ZafiroPath path) => fs.DeleteFile(path);
    public Task<Result> DeleteDirectory(ZafiroPath path) => fs.DeleteDirectory(path);
    public Task<Result<bool>> ExistDirectory(ZafiroPath path) => fs.ExistDirectory(path);
    public Task<Result> CreateFile(ZafiroPath path) => fs.CreateFile(path);
    public IObservable<byte> GetFileContents(ZafiroPath path) => fs.GetFileContents(path);
    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes) => fs.SetFileContents(path, bytes);
    public Task<Result> CreateDirectory(ZafiroPath path) => fs.CreateDirectory(path);
    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path) => fs.GetFileProperties(path);
    public Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path) => fs.GetDirectoryProperties(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path) => fs.GetFilePaths(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path) => fs.GetDirectoryPaths(path);
    public IObservable<FileSystemChange> Changed => fs.Changed;
}