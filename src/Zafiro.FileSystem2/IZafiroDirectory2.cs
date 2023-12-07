using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem2;

public interface IZafiroDirectory2
{
    Task<Result> Create();
    Task<Result<bool>> Exists { get; }
}