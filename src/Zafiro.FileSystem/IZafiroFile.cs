using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IZafiroFile
{
    ZafiroPath Path { get; }
    Task<Result<long>> Size();
    Task<Result<Stream>> GetContents();
    Task<Result> SetContents(Stream stream);
    Task<Result> Delete();
}