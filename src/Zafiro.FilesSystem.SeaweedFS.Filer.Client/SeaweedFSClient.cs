using System.Net;
using System.Runtime.Caching;
using System.Text.Json;
using Refit;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class SeaweedFSClient : ISeaweedFS
{
    private readonly HttpClient httpClient;
    private readonly ISeaweedApi inner;
    private readonly MemoryCache fileMetadatas = MemoryCache.Default;

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

    public async Task<RootDirectory> GetContents(string directoryPath, CancellationToken cancellationToken = default)
    {
        var contents = await inner.GetContents(directoryPath, cancellationToken);
        var files = contents.Entries?.OfType<FileMetadata>() ?? Enumerable.Empty<FileMetadata>();

        foreach (var file in files)
        {
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5),
            };
            fileMetadatas.Add(file.FullPath, file, policy);
        }

        return contents;
    }

    public async Task Upload(string path, Stream stream, CancellationToken cancellationToken = default)
    {
        //var ms = new MemoryStream();
        //await stream.CopyToAsync(ms, cancellationToken);

        //ms.Position = 0;
        //await inner.Upload(path, ms, cancellationToken);
        await inner.Upload(path, stream, cancellationToken);
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

    public async Task<FileMetadata> GetFileMetadata(string path, CancellationToken cancellationToken = default)
    {
        if (fileMetadatas.Get(path) is FileMetadata metadata)
        {
            return metadata;
        }

        return await inner.GetFileMetadata(path, cancellationToken);
    }

    public async Task<bool> PathExists(string path)
    {
        try
        {
            await GetFileMetadata(path);
        }
        catch (ApiException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        return true;
    }
}