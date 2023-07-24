namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public interface ISeaweedFS
{
    Task<RootDirectory> GetContents(string directoryPath);
    Task Upload(string path, Stream stream);
    Task CreateFolder(string directoryPath);
    Task<Stream> GetFileContent(string filePath);
    Task DeleteFile(string filePath);
    Task DeleteFolder(string directoryPath);
}