using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class ZafiroFile : IZafiroFile
{
    private readonly IFileSystemRoot fileSystemRoot;

    public ZafiroFile(ZafiroPath path, IFileSystemRoot fileSystemRoot)
    {
        Path = path;
        this.fileSystemRoot = fileSystemRoot;
    }

    public IObservable<byte> Contents => fileSystemRoot.GetFileContents(Path);
    public Task<Result<bool>> Exists => fileSystemRoot.ExistFile(Path);
    public ZafiroPath Path { get; }
    public Task<Result> Delete() => fileSystemRoot.DeleteFile(Path);
    public Task<Result> SetContents(IObservable<byte> contents) => fileSystemRoot.SetFileContents(Path, contents);
    public Task<Result<FileProperties>> Properties => fileSystemRoot.GetFileProperties(Path);
}