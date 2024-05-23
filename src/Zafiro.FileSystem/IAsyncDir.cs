using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IAsyncDir : INode
{
    public Task<Result<IEnumerable<INode>>> Children();
}