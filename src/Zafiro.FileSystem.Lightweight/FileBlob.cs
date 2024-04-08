using System.IO.Abstractions;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class FileBlob(IFileInfo file): IBlob
{
    public Func<Task<Result<Stream>>> StreamFactory => () => Task.FromResult(Result.Try(() => (Stream) file.OpenRead()));
    public string Name => file.Name;
}