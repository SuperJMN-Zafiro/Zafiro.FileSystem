using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS.Mutable;

public class Directory : IMutableDirectory
{
    public SeaweedFSDirectory Inner { get; }
    public ISeaweedFS SeaweedFS { get; }

    public Directory(SeaweedFSDirectory inner)
    {
        Inner = inner;
        SeaweedFS = inner.SeaweedFS;
    }

    public string Name => Inner.Name;
    public Task<Result<IEnumerable<INode>>> Children() => Inner.Children();

    public bool IsHidden => false;

    public Task<Result<bool>> Exists()
    {
        throw new NotImplementedException();
    }

    public Task<Result> Create()
    {
        return SeaweedFS.CreateFolder(Inner.Path);
    }

    public Task<Result<IEnumerable<IMutableNode>>> MutableChildren()
    {
        throw new NotImplementedException();
    }

    public Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Delete()
    {
        throw new NotImplementedException();
    }
}