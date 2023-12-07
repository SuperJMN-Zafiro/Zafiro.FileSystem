using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Evolution;

public interface IZafiroDirectory2
{
    ZafiroPath Path { get; }
    Task<Result<bool>> Exists { get; }
    Task<Result> Create();
}