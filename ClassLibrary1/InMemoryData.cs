using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public class InMemoryData : IData
{
    public string Name { get; }

    public InMemoryData(string name, Func<Task<Result<Stream>>> streamFactory)
    {
        Name = name;
        StreamFactory = streamFactory;
    }

    public Func<Task<Result<Stream>>> StreamFactory { get; }
}