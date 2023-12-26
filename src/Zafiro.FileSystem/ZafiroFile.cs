using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class ZafiroFile : IZafiroFile
{
    public ZafiroFile(ZafiroPath path, IFileSystemRoot fileSystemRoot)
    {
        Path = path;
        FileSystem = fileSystemRoot;
    }

    public IFileSystemRoot FileSystem { get; }
    public IObservable<byte> Contents => FileSystem.GetFileContents(Path);
    public Task<Result<bool>> Exists => FileSystem.ExistFile(Path);
    public ZafiroPath Path { get; }
    public Task<Result<IDictionary<ChecksumKind, byte[]>>> Hashes => FileSystem.GetChecksums(Path);
    public Task<Result> Delete() => FileSystem.DeleteFile(Path);

    public Task<Result> SetContents(IObservable<byte> contents) => FileSystem.SetFileContents(Path, contents);

    public Task<Result<FileProperties>> Properties => FileSystem.GetFileProperties(Path);
}