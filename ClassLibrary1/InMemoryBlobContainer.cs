using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public class InMemoryBlobContainer : IBlobContainer
{
    private readonly IEnumerable<IBlob> files;
    private readonly IEnumerable<IBlobContainer> children;

    public InMemoryBlobContainer(IEnumerable<IBlob> files, IEnumerable<IBlobContainer> children) : this("", files, children)
    {
    }

    public InMemoryBlobContainer(string name, IEnumerable<IBlob> files, IEnumerable<IBlobContainer> children)
    {
        Name = name;
        this.files = files;
        this.children = children;
    }

    public string Name { get; }
    public Task<Result<IEnumerable<IBlob>>> Blobs() => Task.FromResult(Result.Success(files));

    public Task<Result<IEnumerable<IBlobContainer>>> Children() => Task.FromResult(Result.Success(children));

}