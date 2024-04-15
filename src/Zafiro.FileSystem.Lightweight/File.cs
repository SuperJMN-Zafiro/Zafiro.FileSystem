using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class File : IFile
{
    private readonly Func<Task<Result<Stream>>> streamFactory;

    public File(string name, Func<Task<Result<Stream>>> streamFactory)
    {
        this.streamFactory = streamFactory;
        Name = name;
    }

    public Task<Result<Stream>> Open() => streamFactory();

    public string Name { get; }
}