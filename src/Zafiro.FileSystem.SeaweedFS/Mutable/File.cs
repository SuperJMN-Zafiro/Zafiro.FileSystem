using System.Reactive.Concurrency;
using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS.Mutable;

public class File(string name, ISeaweedFS seaweedFS) : IMutableFile
{
    public ISeaweedFS SeaweedFS { get; } = seaweedFS;
    public string Name { get; } = name;
    public bool IsHidden => false;
    public Task<Result> Create()
    {
        throw new NotImplementedException();
    }

    public Task<Result> SetContents(IData data, CancellationToken cancellationToken = default, IScheduler? scheduler = null)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IData>> GetContents()
    {
        throw new NotImplementedException();
    }

    public Task<Result> Delete()
    {
        throw new NotImplementedException();
    }
}