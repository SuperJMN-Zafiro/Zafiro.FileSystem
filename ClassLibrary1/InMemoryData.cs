using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public class InMemoryData : IData
{

    public InMemoryData(Func<Task<Result<Stream>>> streamFactory)
    {
        StreamFactory = streamFactory;
    }

    public Func<Task<Result<Stream>>> StreamFactory { get; }
}