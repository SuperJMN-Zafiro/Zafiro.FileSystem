using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class Directory : IDirectory
{
    private readonly Task<Result<IEnumerable<IFile>>> files;
    private readonly Task<Result<IEnumerable<IDirectory>>> directories;

    public Directory(string name, IEnumerable<IFile> files, IEnumerable<IDirectory> children) 
        : this(name, Task.FromResult(Result.Success(files)),  Task.FromResult(Result.Success(children)))
    {
    }
    
    public Directory(string name, Task<Result<IEnumerable<IFile>>> files, Task<Result<IEnumerable<IDirectory>>> directories)
    {
        Name = name;
        this.files = files;
        this.directories = directories;
    }

    public string Name { get; }

    public Task<Result<IEnumerable<IFile>>> Files() => files;

    public Task<Result<IEnumerable<IDirectory>>> Directories() => directories;
}