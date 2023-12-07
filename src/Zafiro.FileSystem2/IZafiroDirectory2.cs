using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.FileSystem2;

public interface IZafiroDirectory2
{
    ZafiroPath Path { get; }
    Task<Result<bool>> Exists { get; }
    Task<Result> Create();
}