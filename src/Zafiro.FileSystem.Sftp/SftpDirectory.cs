﻿using CSharpFunctionalExtensions;
using Renci.SshNet;

namespace Zafiro.FileSystem.Sftp;

public class SftpDirectory : IZafiroDirectory
{
    private readonly SftpClient client;

    public SftpDirectory(string path, SftpClient client)
    {
        Path = path;
        this.client = client;
    }

    public ZafiroPath Path { get; }

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
    {
        return Result
            .Try(() => client.ListDirectoryAsync(Path), ex => ErrorHandler(Path, ex))
            .Map(files => files
                .Where(s => s.IsDirectory)
                .Where(file => new[] { ".", ".." }.All(x => x != file.Name))
                .Select(file => (IZafiroDirectory)new SftpDirectory(file.FullName, client)));
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles()
    {
        return Result
            .Try(() => client.ListDirectoryAsync(Path), exception => ErrorHandler(Path, exception))
            .Map(files => files
                .Where(s => !s.IsDirectory)
                .Select(file => (IZafiroFile)new SftpFile(file.FullName, client)));
    }

    private string ErrorHandler(ZafiroPath zafiroPath, Exception exception)
    {
        return $"{exception.Message} for path '{zafiroPath}'";
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath destPath)
    {
        return Task.FromResult(Result.Success((IZafiroFile)new SftpFile(destPath, client)));
    }
}