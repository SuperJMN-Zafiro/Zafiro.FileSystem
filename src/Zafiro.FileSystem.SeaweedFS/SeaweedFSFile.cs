using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.DataModel;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFSFile : IFile
{
    public SeaweedFSFile(FileMetadata file, ISeaweedFS api)
    {
        File = file;
        Api = api;
        var chunkedData = ReactiveData.Chunked(() => api.GetFileContents(file.FullPath, CancellationToken.None));
        Data = new Data(chunkedData, file.FileSize);
    }

    public FileMetadata File { get; }
    public ISeaweedFS Api { get; }

    public Data Data { get; }
    public IObservable<byte[]> Bytes => Data.Bytes;
    public long Length => Data.Length;
    public string Name => ((ZafiroPath) File.FullPath).Name();

    public static Task<Result<SeaweedFSFile>> From(ZafiroPath path, ISeaweedFS fs)
    {
        return Result.Try(() => fs.GetFileMetadata(path), ex => RefitBasedAccessExceptionHandler.HandlePathAccessError(path, ex, Maybe<ILogger>.None))
            .Map(metadata => new SeaweedFSFile(metadata, fs));
    }

    public override string ToString() => File.FullPath;
}