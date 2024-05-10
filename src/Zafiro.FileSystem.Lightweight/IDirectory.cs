using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public interface IDirectory : INode
{
    public Task<Result<IEnumerable<INode>>> Children();
}