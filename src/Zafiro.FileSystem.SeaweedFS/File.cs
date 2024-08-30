using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.Reactive;

namespace Zafiro.FileSystem.SeaweedFS;

public class File(ZafiroPath path, ISeaweedFS seaweedFS) : IMutableFile
{
    public ZafiroPath Path { get; } = path;
    public ISeaweedFS SeaweedFS { get; } = seaweedFS;

    public string Name => Path.Name();
    public bool IsHidden => false;

    public Task<Result> SetContents(IData data, CancellationToken cancellationToken = default, IScheduler? scheduler = null)
    {
        return SeaweedFS.Upload(Path, data.Bytes.ToStream(), cancellationToken);
    }

    public Task<Result<IData>> GetContents()
    {
        return seaweedFS.GetFileMetadata(Path).Map(GetData);
    }

    private IData GetData(FileMetadata metadata)
    {
        var obs = Observable.FromAsync(
            () => SeaweedFS.GetFileContents(Path)).Select(result =>
        {
            var observable = result.Match<IObservable<byte[]>, Stream>(stream => stream.ReadToEndObservable(), s => Observable.Throw<byte[]>(new Exception(s)));
            return observable;
        });
        
        return new Data(obs.SelectMany(observable => observable), metadata.FileSize);
    }

    public override string ToString()
    {
        return Path;
    }

    public Task<Result<bool>> Exists()
    {
        var exists = SeaweedFS.GetFileMetadata(Path)
            .Match(
                _ => Result.Success(true),
                err =>
                {
                    if (err.Contains("404"))
                    {
                        return Result.Success(false);
                    }
                    
                    return Result.Failure<bool>(err);
                });
        return exists;
    }
}