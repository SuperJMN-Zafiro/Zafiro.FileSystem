using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Core;

public interface IHeavyDirectory : INode
{
    public Task<Result<IEnumerable<INode>>> Children();
}