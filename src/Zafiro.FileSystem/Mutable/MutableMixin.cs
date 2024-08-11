using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CSharpFunctionalExtensions;
using DynamicData;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;
using File = Zafiro.FileSystem.Readonly.File;

namespace Zafiro.FileSystem.Mutable;

public static class MutableMixin
{
    public static IObservable<IChangeSet<IMutableFile, string>> Files(this IMutableDirectory directory) =>
        directory.Children.Filter(x => x is IMutableFile).Cast(node => (IMutableFile)node);
    
    public static IObservable<IChangeSet<IMutableDirectory, string>> Directories(this IMutableDirectory directory) =>
        directory.Children.Filter(x => x is IMutableDirectory).Cast(node => (IMutableDirectory)node);
    
    public static Task<Result<IFile>> AsReadOnly(this IMutableFile file)
    {
        return file.GetContents().Map(data => (IFile)new File(file.Name, data));
    }

    public static Task<Result> CreateFileWithContents(this IMutableDirectory directory, string name, IData data)
    {
        return directory.CreateFile(name)
            .Bind(f => f.SetContents(data));
    }

    public static async Task<Maybe<IMutableFile>> GetFile(this IMutableDirectory directory, ZafiroPath path)
    {
        var files = await FilesSnapshot(directory);
        return files.TryFirst(file => file.Name == path.Name());
    }

    private static Task<IReadOnlyCollection<IMutableFile>> FilesSnapshot(this IMutableDirectory directory)
    {
        return directory.Files().ToCollection().ToTask();
    }


    public static Task<Result<IMutableFile>> GetFile(this IMutableFileSystem fileSystem, ZafiroPath path)
    {
        return path.Parent()
            .ToResult($"Cannot get the directory of path '{path}")
            .Map(fileSystem.GetDirectory)
            .Bind(dir => dir.GetFile(path.Name()).ToResult($"Not found {path.Name()}"));
    }

    public static string GetKey(this IMutableNode node)
    {
        return node switch
        {
            IMutableDirectory mutableDirectory => mutableDirectory.Name + "/",
            IMutableFile mutableFile => mutableFile.Name,
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };
    }
}