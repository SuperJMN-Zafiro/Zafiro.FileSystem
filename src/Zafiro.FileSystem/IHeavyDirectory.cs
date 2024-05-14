using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IHeavyDirectory : INode
{
    public Task<Result<IEnumerable<INode>>> Children();
}