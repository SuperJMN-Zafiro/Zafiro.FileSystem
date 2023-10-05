using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IZafiroFile
{
    ZafiroPath Path { get; }
    Task<Result<long>> Size();
    Task<Result<bool>> Exists();
    Task<Result<Stream>> GetContents(CancellationToken cancellationToken = default);
    Task<Result> SetContents(Stream stream, CancellationToken cancellationToken = default);
    Task<Result> Delete(CancellationToken cancellationToken = default);
}