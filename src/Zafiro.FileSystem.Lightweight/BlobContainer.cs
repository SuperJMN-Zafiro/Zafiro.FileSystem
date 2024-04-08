using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class BlobContainer : IBlobContainer
{
    private readonly IEnumerable<IBlob> files;
    private readonly IEnumerable<IBlobContainer> children;

    public BlobContainer(IEnumerable<IBlob> files, IEnumerable<IBlobContainer> children) : this("", files, children)
    {
    }

    public BlobContainer(string name, IEnumerable<IBlob> files, IEnumerable<IBlobContainer> children)
    {
        Name = name;
        this.files = files;
        this.children = children;
    }

    public string Name { get; }
    public Task<Result<IEnumerable<IBlob>>> Blobs() => Task.FromResult(Result.Success(files));

    public Task<Result<IEnumerable<IBlobContainer>>> Children() => Task.FromResult(Result.Success(children));

}