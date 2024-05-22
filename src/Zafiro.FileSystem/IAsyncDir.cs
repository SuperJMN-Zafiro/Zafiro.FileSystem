using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.VNext.Interfaces;

public interface IAsyncDir : INode
{
    public Task<Result<IEnumerable<INode>>> Children();
}