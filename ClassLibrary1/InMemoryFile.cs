using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public class InMemoryFile : IFile
{
    private readonly Maybe<IDirectory> parent;
    public string Name { get; }

    public InMemoryFile(string name) : this(name, Maybe<IDirectory>.None) { }

    public InMemoryFile(string name, Maybe<IDirectory> parent)
    {
        this.parent = parent;
        Name = name;
    }

    public Task<Result<Maybe<IDirectory>>> Parent => Task.FromResult(Result.Success(parent));

    public Func<Task<Result<Stream>>> StreamFactory => throw new NotImplementedException();
}