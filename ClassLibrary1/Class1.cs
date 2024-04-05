using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public interface IDirectory
{
    public string Name { get; }
    Task<Result<IEnumerable<IFile>>> GetFiles();
    Task<Result<IEnumerable<IDirectory>>> GetDirectories();
    Task<Result<Maybe<IDirectory>>> Parent { get; }
}

public class InMemoryDirectory : IDirectory
{
    private readonly Maybe<IDirectory> parent;
    private readonly Func<IDirectory, IEnumerable<IFile>> files;
    private readonly Func<IDirectory, IEnumerable<IDirectory>> directories;

    public InMemoryDirectory(string name, Maybe<IDirectory> parent, Func<IDirectory, IEnumerable<IFile>> files, Func<IDirectory, IEnumerable<IDirectory>> directories)
    {
        Name = name;
        this.parent = parent;
        this.files = files;
        this.directories = directories;
    }

    public string Name { get; }
    public Task<Result<IEnumerable<IFile>>> GetFiles() => Task.FromResult(Result.Success(files(this)));

    public Task<Result<IEnumerable<IDirectory>>> GetDirectories() => Task.FromResult(Result.Success(directories(this)));

    public Task<Result<Maybe<IDirectory>>> Parent => Task.FromResult(Result.Success(parent));
}

public interface IFile
{
    public string Name { get; }
    public Task<Result<Maybe<IDirectory>>> Parent { get; }
}

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
}