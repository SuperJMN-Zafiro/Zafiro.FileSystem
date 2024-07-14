using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Mutable;

public static class MutableDirectoryMixin
{
    public static Task<Result<IEnumerable<IMutableFile>>> MutableFiles(this IMutableDirectory directory) =>
        directory.MutableChildren().Map(x => x.OfType<IMutableFile>());
    
    public static IObservable<Result<IEnumerable<IMutableFile>>> MutableFilesObs(this IMutableDirectory directory) =>
        directory.ChildrenProp.Map(nodes => nodes.OfType<IMutableFile>());
    
    public static IObservable<Result<IEnumerable<IMutableDirectory>>> MutableDirectoriesObs(this IMutableDirectory directory) =>
        directory.ChildrenProp.Map(nodes => nodes.OfType<IMutableDirectory>());
    
    public static Task<Result<Maybe<IMutableFile>>> MutableFile(this IMutableDirectory directory, string fileName)
    {
        return directory.MutableChildren()
            .Map(x => x.OfType<IMutableFile>()
                .TryFirst(x => x.Name == fileName));
    }

    public static Task<Result<IEnumerable<IMutableDirectory>>> MutableDirectories(this IMutableDirectory directory) =>
        directory.MutableChildren().Map(x => x.OfType<IMutableDirectory>());
}