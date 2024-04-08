using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public interface IGetStream
{
    public Func<Task<Result<Stream>>> StreamFactory { get; }
}