using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Readonly;
using File = Zafiro.FileSystem.Readonly.File;

namespace Zafiro.FileSystem.Mutable;

public static class MutableMixin
{
    public static IObservable<Result<IEnumerable<IMutableFile>>> MutableFilesObs(this IMutableDirectory directory) =>
        directory.Children.Map(nodes => nodes.OfType<IMutableFile>());
    
    public static IObservable<Result<IEnumerable<IMutableDirectory>>> MutableDirectoriesObs(this IMutableDirectory directory) =>
        directory.Children.Map(nodes => nodes.OfType<IMutableDirectory>());
    
    public static IObservable<Result<Maybe<IMutableFile>>> MutableFile(this IMutableDirectory directory, string fileName)
    {
        return directory.MutableFilesObs()
            .Map(files => files.TryFirst(file => file.Name == fileName));
    }

    public static Task<Result<IFile>> AsReadOnly(this IMutableFile file)
    {
        return file.GetContents().Map(data => (IFile)new File(file.Name, data));
    }
}