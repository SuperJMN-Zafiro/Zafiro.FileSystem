using System.Reactive.Concurrency;
using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS.Mutable;

public class File : IMutableFile
{
    public File(string path, ISeaweedFS seaweedFS)
    {
        SeaweedFS = seaweedFS;
        Path = path;
    }

    public string Path { get; }

    public ISeaweedFS SeaweedFS { get; }
    public string Name { get; }
    public bool IsHidden => false;
    public Task<Result<bool>> Exists()
    {
        var exists = SeaweedFS.GetFileMetadata(Path)
            .Match(
                _ => Result.Success(true), 
                err =>
                {
                    if (err.Contains("404"))
                        return Result.Success(false);
                    else
                        return Result.Failure<bool>(err);
                });
        return exists;
    }

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