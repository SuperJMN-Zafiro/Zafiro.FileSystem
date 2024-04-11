using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public interface IDirectory
{
    public string Name { get; }
    Task<Result<IEnumerable<IFile>>> Files();
    Task<Result<IEnumerable<IDirectory>>> Directories();
}