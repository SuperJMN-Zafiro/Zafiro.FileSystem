using System.Reflection;
using Refit;
using System.Text.Json;
using System.Text;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class SeaweedFSClient : ISeaweedFS
{
    private readonly HttpClient httpClient;
    private readonly ISeaweedApi inner;

    public SeaweedFSClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        inner = RestService.For<ISeaweedApi>(httpClient, new RefitSettings()
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions()
            {
                Converters =
                {
                    new FileSystemEntryConverter()
                }
            }),
        });
    }

    public Task<RootDirectory> GetContents(string directoryPath)
    {
        return inner.GetContents(directoryPath);
    }

    public Task Upload(string path, Stream stream)
    {
        return inner.Upload(path, stream);
    }

    public async Task CreateFolder(string directoryPath)
    {
        await inner.CreateFolder(directoryPath);
    }

    public Task DeleteFolder(string directoryPath)
    {
        return inner.DeleteFolder(directoryPath);
    }

    public Task<Stream> GetFileContent(string filePath)
    {
        return httpClient.GetStreamAsync(filePath);
    }

    public Task DeleteFile(string filePath)
    {
        return inner.DeleteFile(filePath);
    }

    public Task GetFileMetadata(string path)
    {
        return inner.GetFileMetadata(path);
    }
}