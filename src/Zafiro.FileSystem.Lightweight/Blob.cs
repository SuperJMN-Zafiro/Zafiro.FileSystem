using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class Blob : IBlob
{

    public Blob(string name, Func<Task<Result<Stream>>> streamFactory)
    {
        StreamFactory = streamFactory;
        Name = name;
    }

    public Func<Task<Result<Stream>>> StreamFactory { get; }

    public string Name { get; }
}