using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public class InMemoryDataTree : IDataTree
{
    private readonly IEnumerable<IData> files;
    private readonly IEnumerable<IDataTree> directories;

    public InMemoryDataTree(IEnumerable<IData> files, IEnumerable<IDataTree> directories) : this("", files, directories)
    {
    }

    public InMemoryDataTree(string name, IEnumerable<IData> files, IEnumerable<IDataTree> directories)
    {
        Name = name;
        this.files = files;
        this.directories = directories;
    }

    public string Name { get; }
    public Task<Result<IEnumerable<IData>>> GetFiles() => Task.FromResult(Result.Success(files));

    public Task<Result<IEnumerable<IDataTree>>> GetDirectories() => Task.FromResult(Result.Success(directories));

}