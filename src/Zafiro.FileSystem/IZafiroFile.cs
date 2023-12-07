using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IZafiroFile
{
    IObservable<byte> Contents { get; }
    Task<Result<bool>> Exists { get; }
    ZafiroPath Path { get; }
    Task<Result<FileProperties>> Properties { get; }
    Task<Result> Delete();
    Task<Result> SetContents(IObservable<byte> contents);
}