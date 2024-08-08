using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.DataModel;
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

    public static Task<Result> CreateFileWithContents(this IMutableDirectory directory, string name, IData data)
    {
        return directory.CreateFile(name)
            .Bind(f => f.SetContents(data));
    }
}