using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using IFile = Zafiro.FileSystem.IFile;

namespace Zafiro.FileSystem;

public class SystemIOHeavyDirectory(IDirectoryInfo directory) : IHeavyDirectory
{
    public string Name => directory.Name;
    public Task<Result<IEnumerable<IFile>>> Files() => Task.FromResult(Result.Try(() => directory.GetFiles().Select(info => (IFile)new SystemIOFile(info))));
    public Task<Result<IEnumerable<INode>>> Children() => Task.FromResult(Result.Try(() =>
    {
        var file = directory.GetFiles().Select(info => (INode) new SystemIOFile(info));
        var dirs = directory.GetDirectories().Select(info => (INode) new SystemIOHeavyDirectory(info));
        return file.Concat(dirs);
    }));

    public Task<Result<IEnumerable<IHeavyDirectory>>> Directories() => Task.FromResult(Result.Try(() => directory.GetDirectories().Select(info => (IHeavyDirectory)new SystemIOHeavyDirectory(info))));
}