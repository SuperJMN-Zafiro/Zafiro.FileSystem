using System.IO.Abstractions;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class DirectorioIODirectory(Maybe<string> name, IDirectoryInfo directory) : IDirectory
{
    public string Name => name.GetValueOrDefault(directory.Name);
    public Task<Result<IEnumerable<IFile>>> Files() => Task.FromResult(Result.Try(() => directory.GetFiles().Select(info => (IFile)new SystemIOFile(info))));
    public Task<Result<IEnumerable<IDirectory>>> Directories() => Task.FromResult(Result.Try(() => directory.GetDirectories().Select(info => (IDirectory)new DirectorioIODirectory(info.Name, info))));
}