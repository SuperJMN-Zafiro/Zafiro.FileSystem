using Refit;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

[Headers("Accept: application/json")]
public interface ISeaweedApi
{
    [Get("/{directoryPath}/?pretty=y")]
    Task<RootDirectory> GetContents(string directoryPath);

    [Get("/{filePath}?metadata=true&pretty=y")]
    Task<File> GetFileMetadata(string filePath);

    [Multipart]
    [Post("/{path}")]
    Task Upload(string path, Stream stream);

    [Multipart]
    [Post("/{directoryPath}/")]
    Task CreateFolder(string directoryPath);

    [Delete("/{filePath}")]
    Task DeleteFile(string filePath);

    [Delete("/{directoryPath}?recursive=true")]
    Task DeleteFolder(string directoryPath);
}