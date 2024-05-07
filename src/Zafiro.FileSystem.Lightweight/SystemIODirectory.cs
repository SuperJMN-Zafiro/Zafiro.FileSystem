using System.IO.Abstractions;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class SystemIODirectory(IDirectoryInfo directory) : IDirectory
{
    public string Name => directory.Name;
    public Task<Result<IEnumerable<IFile>>> Files() => Task.FromResult(Result.Try(() => directory.GetFiles().Select(info => (IFile)new SystemIOFile(info))));
    public Task<Result<IEnumerable<INode>>> Children() => Task.FromResult(Result.Try(() =>
    {
        var file = directory.GetFiles().Select(info => (INode) new SystemIOFile(info));
        var dirs = directory.GetDirectories().Select(info => (INode) new SystemIODirectory(info));
        return file.Concat(dirs);
    }));

    public Task<Result<IEnumerable<IDirectory>>> Directories() => Task.FromResult(Result.Try(() => directory.GetDirectories().Select(info => (IDirectory)new SystemIODirectory(info))));
}