using CSharpFunctionalExtensions;
using Renci.SshNet;
using Renci.SshNet.Common;
using Serilog;

namespace Zafiro.FileSystem.Sftp;

public class SftpFileSystem : IFileSystem, IDisposable
{
    private readonly Maybe<ILogger> logger;
    private readonly SftpClient sftpClient;

    private SftpFileSystem(SftpClient sftpClient, Maybe<ILogger> logger)
    {
        this.sftpClient = sftpClient;
        this.logger = logger;
    }

    public void Dispose()
    {
        sftpClient.ErrorOccurred -= ClientOnErrorOccurred(logger);
        sftpClient.Dispose();
    }

    public Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroDirectory>(() => new SftpDirectory(path, sftpClient, this)));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroFile>(() => new SftpFile(path, sftpClient)));
    }

    public Task<Result<ZafiroPath>> GetRoot()
    {
        return Task.FromResult<Result<ZafiroPath>>(ZafiroPath.Empty);
    }

    public static Task<Result<SftpFileSystem>> Create(string host, int port, string username, string password, Maybe<ILogger> logger)
    {
        return Task.FromResult(Result.Try(() =>
        {
            var client = new SftpClient(host, port, username, password);
            client.Connect();
            client.ErrorOccurred += ClientOnErrorOccurred(logger);
            return new SftpFileSystem(client, logger);
        }, ex => $"Can't create SFTP client to connect to {host}. Reason {ex.Message}."));
    }

    private static EventHandler<ExceptionEventArgs> ClientOnErrorOccurred(Maybe<ILogger> logger)
    {
        return (_, args) => logger.Execute(l => l.Error(args.Exception, "Error in SFTP client"));
    }
}