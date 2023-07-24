using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IZafiroFile
{
    ZafiroPath Path { get; }
    Task<Result<Stream>> GetContents();
    Task<Result> SetContents(Stream stream);
    Task<Result> Delete();
}