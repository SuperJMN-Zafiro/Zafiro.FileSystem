using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IFileCompareStrategy
{
    Task<Result<bool>> Compare(IZafiroFile one, IZafiroFile another);
}