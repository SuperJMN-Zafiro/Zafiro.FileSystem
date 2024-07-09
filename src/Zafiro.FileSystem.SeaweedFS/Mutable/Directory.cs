using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.Readonly;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using File = Zafiro.FileSystem.Readonly.File;

namespace Zafiro.FileSystem.SeaweedFS.Mutable;

public class Directory : IMutableDirectory
{
    public SeaweedFSDirectory Directory { get; }
    public ISeaweedFS SeaweedFS { get; }

    public Directory(SeaweedFSDirectory directory, ISeaweedFS seaweedFS)
    {
        Directory = directory;
        SeaweedFS = seaweedFS;
    }

    public string Name => Directory.Name;
    public Task<Result<IEnumerable<INode>>> Children() => Directory.Children();

    public bool IsHidden => false;
    
    public Task<Result> Create()
    {
        return Result.Try(() => SeaweedFS.CreateFolder(Directory.Path), exception => RefitBasedAccessExceptionHandler.HandlePathAccessError(Directory.Path, exception, Maybe<ILogger>.None));
    }

    public Task<Result<IEnumerable<IMutableNode>>> MutableChildren()
    {
        throw new NotImplementedException();
    }

    public Task<Result> AddOrUpdate(IFile data, ISubject<double>? progress = null)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IMutableFile>> Get(string name)
    {
        return Result.Success((IMutableFile)new File(name, SeaweedFS));
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