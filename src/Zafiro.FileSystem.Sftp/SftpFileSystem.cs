using CSharpFunctionalExtensions;
using Renci.SshNet;

namespace Zafiro.FileSystem.Sftp;

public class SftpFileSystem : IFileSystem, IDisposable
{
    private readonly SftpClient scpClient;

    private SftpFileSystem(SftpClient scpClient)
    {
        this.scpClient = scpClient;
    }

    public void Dispose()
    {
        scpClient.Dispose();
    }

    public Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroDirectory>(() => new SftpDirectory(path, scpClient)));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroFile>(() => new SftpFile(path, scpClient)));
    }

    public static Task<Result<SftpFileSystem>> Create(string host, int port, string username, string password)
    {
        return Task.FromResult(Result.Try(() =>
        {
            var client = new SftpClient(host, port, username, password);
            client.Connect();
            return new SftpFileSystem(client);
        }));
    }
}