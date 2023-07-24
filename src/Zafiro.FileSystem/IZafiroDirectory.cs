using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IZafiroDirectory
{
    ZafiroPath Path { get; }
    Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories();
    Task<Result<IEnumerable<IZafiroFile>>> GetFiles();
    Task<Result<IZafiroFile>> GetFile(ZafiroPath destPath);
}