using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.FileSystem2;

public interface IZafiroFile2
{
    IObservable<byte> Contents { get; }
    Task<Result<bool>> Exists { get; }
    ZafiroPath Path { get; }
    Task<Result> Delete();
}