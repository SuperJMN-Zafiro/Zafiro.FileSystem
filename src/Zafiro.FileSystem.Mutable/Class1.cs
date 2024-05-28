using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Lightweight;

namespace Zafiro.FileSystem.Mutable;

public class Class1
{

}

public interface IMutableDirectory : IMutableNode, IAsyncDir
{
    Task<Result<IEnumerable<IMutableNode>>> MutableChildren();

}

public interface IMutableFile : IFile, IMutableNode
{
}

public interface IMutableNode : INode
{
}