using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public interface IContainer
{
    Task<Result<IEnumerable<IFile>>> Files();
    Task<Result<IEnumerable<IDirectory>>> Directories();
}

public interface IDirectory : IContainer, INamed
{
    Task<Result<IEnumerable<IFile>>> Files();
    Task<Result<IEnumerable<IDirectory>>> Directories();
}