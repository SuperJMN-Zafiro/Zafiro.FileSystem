using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public interface IStreamOpen
{
    public Func<Task<Result<Stream>>> Open { get; }
}