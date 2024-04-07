using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public class InMemoryDataTree : IDataTree
{
    private readonly IEnumerable<DataEntry> files;
    private readonly IEnumerable<DataNode> directories;

    public InMemoryDataTree(IEnumerable<DataEntry> files, IEnumerable<DataNode> directories) : this("", files, directories)
    {
    }

    public InMemoryDataTree(string name, IEnumerable<DataEntry> files, IEnumerable<DataNode> directories)
    {
        Name = name;
        this.files = files;
        this.directories = directories;
    }

    public string Name { get; }
    public Task<Result<IEnumerable<DataEntry>>> GetFiles() => Task.FromResult(Result.Success(files));

    public Task<Result<IEnumerable<DataNode>>> GetDirectories() => Task.FromResult(Result.Success(directories));

}