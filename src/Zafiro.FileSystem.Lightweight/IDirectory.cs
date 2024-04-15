using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public interface IDirectory : INamed
{
    Task<Result<IEnumerable<IFile>>> Files();
    Task<Result<IEnumerable<IDirectory>>> Directories();
}