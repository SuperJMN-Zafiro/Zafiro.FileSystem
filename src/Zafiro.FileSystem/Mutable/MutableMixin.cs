using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Readonly;
using File = Zafiro.FileSystem.Readonly.File;

namespace Zafiro.FileSystem.Mutable;

public static class MutableMixin
{
    public static IObservable<Result<IEnumerable<IMutableFile>>> Files(this IMutableDirectory directory) =>
        directory.Children.Map(nodes => nodes.OfType<IMutableFile>());
    
    public static IObservable<Result<IEnumerable<IMutableDirectory>>> Directories(this IMutableDirectory directory) =>
        directory.Children.Map(nodes => nodes.OfType<IMutableDirectory>());
    
    public static Task<Result<IFile>> AsReadOnly(this IMutableFile file)
    {
        return file.GetContents().Map(data => (IFile)new File(file.Name, data));
    }
}