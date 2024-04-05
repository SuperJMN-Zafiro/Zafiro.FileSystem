using CSharpFunctionalExtensions;

namespace ClassLibrary1;

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