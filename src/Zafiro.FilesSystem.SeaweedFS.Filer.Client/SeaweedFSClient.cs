using System.Text.Json;
using Refit;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class SeaweedFSClient : ISeaweedFS
{
    private readonly HttpClient httpClient;
    private readonly ISeaweedApi inner;

    public SeaweedFSClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        inner = RestService.For<ISeaweedApi>(httpClient, new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                Converters =
                {
                    new FileSystemEntryConverter(),
                },
            }),
        });
    }

    public Task<RootDirectory> GetContents(string directoryPath, CancellationToken cancellationToken = default)
    {
        return inner.GetContents(directoryPath[1..], cancellationToken);
    }

    public Task Upload(string path, Stream stream, CancellationToken cancellationToken = default)
    {
        return inner.Upload(path, stream, cancellationToken);
    }

    public Task CreateFolder(string directoryPath, CancellationToken cancellationToken = default)
    {
        var finalFolder = directoryPath == "/" ? directoryPath : directoryPath[1..];
        return inner.CreateFolder(finalFolder, cancellationToken);
    }

    public Task DeleteFolder(string directoryPath, CancellationToken cancellationToken = default)
    {
        return inner.DeleteFolder(directoryPath, cancellationToken);
    }

    public Task<Stream> GetFileContent(string filePath, CancellationToken cancellationToken = default)
    {
        return httpClient.GetStreamAsync(filePath, cancellationToken);
    }

    public Task DeleteFile(string filePath, CancellationToken cancellationToken = default)
    {
        return inner.DeleteFile(filePath, cancellationToken);
    }

    public Task<File> GetFileMetadata(string path, CancellationToken cancellationToken = default)
    {
        return inner.GetFileMetadata(path, cancellationToken);
    }
}