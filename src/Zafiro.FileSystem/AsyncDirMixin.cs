using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public static class AsyncDirMixin
{
    public static Task<Result<IEnumerable<IFile>>> Files(this IAsyncDir dir) => dir.Children().Map(x => x.OfType<IFile>());
    public static Task<Result<IEnumerable<IDirectory>>> Directories(this IAsyncDir dir) => dir.Children().Map(x => x.OfType<IDirectory>());
}