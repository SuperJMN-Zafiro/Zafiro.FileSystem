using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableDirectory : IMutableNode, IAsyncDir
{
    Task<Result<IEnumerable<IMutableNode>>> MutableChildren();

}