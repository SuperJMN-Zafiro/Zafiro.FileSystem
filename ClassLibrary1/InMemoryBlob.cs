using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public class InMemoryBlob : IBlob
{

    public InMemoryBlob(string name, Func<Task<Result<Stream>>> streamFactory)
    {
        StreamFactory = streamFactory;
        Name = name;
    }

    public Func<Task<Result<Stream>>> StreamFactory { get; }

    public string Name { get; }
}