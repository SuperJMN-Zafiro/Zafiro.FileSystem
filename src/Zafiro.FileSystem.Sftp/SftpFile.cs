using CSharpFunctionalExtensions;
using Renci.SshNet;

namespace Zafiro.FileSystem.Sftp;

public class SftpFile : IZafiroFile
{
    private readonly SftpClient client;

    public SftpFile(ZafiroPath path, SftpClient client)
    {
        Path = path;
        this.client = client;
    }

    public ZafiroPath Path { get; }
    public async Task<Result<Stream>> GetContents()
    {
        var memoryStream = new MemoryStream();
        await client.DownloadFileAsync(Path, memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    public Task<Result> SetContents(Stream stream)
    {
        return Result.Try(async () =>
        {
            EnsureDirectoryExists(Path.Parent());
            await client.UploadFileAsync(Path, stream);
        });
    }

    private void EnsureDirectoryExists(ZafiroPath path)
    {
        if (!client.Exists(path))
        {
            EnsureDirectoryExists(path.Parent());
            client.CreateDirectory(path);
        }
    }

    public Task<Result> Delete()
    {
        return Task.FromResult(Result.Try(() => client.DeleteFile(Path)));
    }
}