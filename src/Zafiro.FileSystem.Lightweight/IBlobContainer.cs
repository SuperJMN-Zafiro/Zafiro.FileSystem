using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public interface IBlobContainer
{
    public string Name { get; }
    Task<Result<IEnumerable<IBlob>>> Blobs();
    Task<Result<IEnumerable<IBlobContainer>>> Children();
}