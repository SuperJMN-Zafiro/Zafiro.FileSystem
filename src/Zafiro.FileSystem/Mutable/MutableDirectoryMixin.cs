using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Mutable;

public static class MutableDirectoryMixin
{
    public static Task<Result<IEnumerable<IMutableFile>>> MutableFiles(this IMutableDirectory directory) =>
        directory.MutableChildren().Map(x => x.OfType<IMutableFile>());
    
    public static Task<Result<Maybe<IMutableFile>>> MutableFile(this IMutableDirectory directory, string fileName)
    {
        return directory.MutableChildren()
            .Map(x => x.OfType<IMutableFile>()
                .TryFirst(x => x.Name == fileName));
    }

    public static Task<Result<IEnumerable<IMutableDirectory>>> MutableDirectories(this IMutableDirectory directory) =>
        directory.MutableChildren().Map(x => x.OfType<IMutableDirectory>());
}