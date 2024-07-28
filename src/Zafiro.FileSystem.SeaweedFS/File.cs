using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using MoreLinq.Extensions;
using Zafiro.CSharpFunctionalExtensions;
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

    public static Task<Result<File>> From(string path, ISeaweedFS seaweedFS)
    {
        var fileMetadataResult = seaweedFS.GetFileMetadata(path);
        var contentStreamResult = seaweedFS.GetFileContents(path, CancellationToken.None);

        return from metadata in fileMetadataResult
            from contentStream in contentStreamResult
            select new File(path, seaweedFS);
    }

    public override string ToString() => Path;
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
        return SeaweedFS.Upload(Path, data.Bytes.ToStream(), cancellationToken);
    }

    public Task<Result<IData>> GetContents()
    {
        return from metadata in SeaweedFS.GetFileMetadata(Path)
            from f in SeaweedFS.GetFileContents(Path)
            select (IData)new Data(f.ReadToEndObservable(), metadata.FileSize);
    }

    public Task<Result> Delete()
    {
        return SeaweedFS.DeleteFile(Path);
    }
}