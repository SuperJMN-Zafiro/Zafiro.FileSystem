using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public interface IDirectory
{
    public string Name { get; }
    Task<Result<IEnumerable<IFile>>> GetFiles();
    Task<Result<IEnumerable<IDirectory>>> GetDirectories();
    Task<Result<Maybe<IDirectory>>> Parent { get; }
}