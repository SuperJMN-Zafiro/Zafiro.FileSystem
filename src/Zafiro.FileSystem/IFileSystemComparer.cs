using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IFileSystemComparer
{
    Task<Result<IEnumerable<Diff>>> Diff(IZafiroDirectory origin, IZafiroDirectory destination);
}