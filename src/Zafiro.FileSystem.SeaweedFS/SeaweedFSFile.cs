using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFSFile : IFile
{
    public string Path { get; }
    public Data Data { get; }

    public SeaweedFSFile()
    {
    }

    private SeaweedFSFile(string path, Data data)
    {
        Path = path;
        Data = data;
    }

    public FileMetadata File { get; }
    public IObservable<byte[]> Bytes => Data.Bytes;
    public long Length => Data.Length;
    public string Name => ((ZafiroPath) File.FullPath).Name();

    public static Task<Result<SeaweedFSFile>> From(string path, ISeaweedFS seaweedFS)
    {
        var fileMetadataResult = seaweedFS.GetFileMetadata(path);
        var contentStreamResult = seaweedFS.GetFileContents(path, CancellationToken.None);

        return from metadata in fileMetadataResult
            from contentStream in contentStreamResult
            select new SeaweedFSFile(path, new Data(ReactiveData.Chunked(() => contentStream), metadata.FileSize));
    }

    public override string ToString() => File.FullPath;
}