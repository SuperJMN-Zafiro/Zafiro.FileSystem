using CSharpFunctionalExtensions;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFile : IZafiroFile
{
    private readonly SeaweedFSClient seaweedStore;

    public SeaweedFile(ZafiroPath path, SeaweedFSClient seaweedStore)
    {
        Path = path;
        this.seaweedStore = seaweedStore;
    }

    public ZafiroPath Path { get; }

    public Task<Result<Stream>> GetContents()
    {
        return Result.Try(() => seaweedStore.GetFileContent(Path));
    }

    public Task<Result> SetContents(Stream stream)
    {
        return Result.Try(() => seaweedStore.Upload(Path, stream));
    }

    public Task<Result> Delete()
    {
        return Result.Try(() => seaweedStore.DeleteFile(Path));
    }

    public override string ToString()
    {
        return Path;
    }
}