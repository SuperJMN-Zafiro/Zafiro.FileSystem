using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IZafiroFile
{
    IObservable<byte> Contents { get; }
    Task<Result<bool>> Exists { get; }
    ZafiroPath Path { get; }
    Task<Result<FileProperties>> Properties { get; }
    Task<Result<IDictionary<ChecksumKind, byte[]>>> Hashes { get; }
    IFileSystemRoot FileSystem { get; }
    Task<Result> Delete();
    Task<Result> SetContents(IObservable<byte> contents);
}