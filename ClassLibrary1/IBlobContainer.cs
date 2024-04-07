using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public interface IBlobContainer
{
    public string Name { get; }
    Task<Result<IEnumerable<IBlob>>> Blobs();
    Task<Result<IEnumerable<IBlobContainer>>> Children();
}