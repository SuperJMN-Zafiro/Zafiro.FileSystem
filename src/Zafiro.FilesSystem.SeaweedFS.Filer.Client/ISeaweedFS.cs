using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public interface ISeaweedFS
{
    Task<RootDirectory> GetContents(string directoryPath, CancellationToken cancellationToken = default);
    Task Upload(string path, Stream stream, CancellationToken cancellationToken = default);
    Task CreateFolder(string directoryPath, CancellationToken cancellationToken = default);
    Task<Stream> GetFileContents(string filePath, CancellationToken cancellationToken = default);
    Task DeleteFile(string filePath, CancellationToken cancellationToken = default);
    Task DeleteFolder(string directoryPath, CancellationToken cancellationToken = default);
    Task<FileMetadata> GetFileMetadata(string path, CancellationToken cancellationToken = default);
    Task<bool> PathExists(string path);
}