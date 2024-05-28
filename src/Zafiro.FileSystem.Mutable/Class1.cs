using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Mutable;

public class Class1
{

}

public interface IMutableDirectory : IMutableNode
{
    Task<Result<IEnumerable<IMutableNode>>> MutableChildren();

}

public interface IMutableFile : IFile, IMutableNode
{
}

public interface IMutableNode
{
}