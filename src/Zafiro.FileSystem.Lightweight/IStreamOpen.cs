using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public interface IStreamOpen
{
    public Task<Result<Stream>> Open();
}