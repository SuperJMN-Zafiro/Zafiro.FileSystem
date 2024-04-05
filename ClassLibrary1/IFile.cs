using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public interface IFile : IGetStream
{
    public string Name { get; }
    public Task<Result<Maybe<IDirectory>>> Parent { get; }
}

public interface IGetStream
{
    public Func<Task<Result<Stream>>> StreamFactory { get; }
}