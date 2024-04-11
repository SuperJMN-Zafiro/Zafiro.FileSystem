using System.IO.Abstractions;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class SystemIOFile(IFileInfo file): IFile
{
    public Func<Task<Result<Stream>>> Open => () => Task.FromResult(Result.Try(() => (Stream) file.OpenRead()));
    public string Name => file.Name;
}