namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public interface ISeaweedFS
{
    Task<RootDirectory> GetContents(string directoryPath, CancellationToken cancellationToken);
    Task Upload(string path, Stream stream, CancellationToken cancellationToken);
    Task CreateFolder(string directoryPath, CancellationToken cancellationToken);
    Task<Stream> GetFileContent(string filePath, CancellationToken cancellationToken);
    Task DeleteFile(string filePath, CancellationToken cancellationToken);
    Task DeleteFolder(string directoryPath, CancellationToken cancellationToken);
}