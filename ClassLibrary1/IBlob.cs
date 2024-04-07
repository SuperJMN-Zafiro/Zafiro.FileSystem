using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public interface IBlob : IGetStream
{
    public string Name { get; }
}

public interface IGetStream
{
    public Func<Task<Result<Stream>>> StreamFactory { get; }
}