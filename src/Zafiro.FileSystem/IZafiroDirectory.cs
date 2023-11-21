using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IZafiroDirectory
{
    public bool IsHidden { get; }
    ZafiroPath Path { get; }
    IFileSystem FileSystem { get; }
    Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories();
    Task<Result<IEnumerable<IZafiroFile>>> GetFiles();
    Task<Result> Delete();
}