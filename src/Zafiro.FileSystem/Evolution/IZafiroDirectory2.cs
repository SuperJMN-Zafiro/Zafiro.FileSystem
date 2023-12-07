using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Evolution;

public interface IZafiroDirectory2
{
    ZafiroPath Path { get; }
    Task<Result<bool>> Exists { get; }
    IFileSystemRoot FileSystem { get; }
    Task<Result> Create();
    Task<Result<IEnumerable<IZafiroFile2>>> GetFiles();
    Task<Result<IEnumerable<IZafiroDirectory2>>> GetDirectories();
    Task<Result> Delete();
}