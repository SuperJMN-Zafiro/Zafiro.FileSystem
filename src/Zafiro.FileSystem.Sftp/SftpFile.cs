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
    public Task<Result<long>> Size()
    {
        return Task.FromResult(Result.Success(client.GetAttributes(Path).Size));
    }

    public Task<Result<Stream>> GetContents()
    {
        return Result.Try(() => Task.FromResult<Stream>(client.OpenRead(Path)));
    }

    public Task<Result> SetContents(Stream stream, CancellationToken cancellationToken)
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