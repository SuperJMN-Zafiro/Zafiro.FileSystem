using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class BlobContainer : IBlobContainer
{
    private readonly Task<Result<IEnumerable<IBlob>>> blobs;
    private readonly Task<Result<IEnumerable<IBlobContainer>>> children;

    public BlobContainer(string name, IEnumerable<IBlob> blobs, IEnumerable<IBlobContainer> children) 
        : this(name, Task.FromResult(Result.Success(blobs)),  Task.FromResult(Result.Success(children)))
    {
    }
    
    public BlobContainer(string name, Task<Result<IEnumerable<IBlob>>> blobs, Task<Result<IEnumerable<IBlobContainer>>> children)
    {
        Name = name;
        this.blobs = blobs;
        this.children = children;
    }

    public string Name { get; }

    public Task<Result<IEnumerable<IBlob>>> Blobs() => blobs;

    public Task<Result<IEnumerable<IBlobContainer>>> Children() => children;
}