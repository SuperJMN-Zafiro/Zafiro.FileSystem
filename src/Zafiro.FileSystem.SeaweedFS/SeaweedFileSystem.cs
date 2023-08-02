using CSharpFunctionalExtensions;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFileSystem : IFileSystem
{
    private readonly SeaweedFSClient seaweedFSClient;

    public SeaweedFileSystem(SeaweedFSClient seaweedFSClient)
    {
        this.seaweedFSClient = seaweedFSClient;
    }

    public Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path)
    {
        return Task.FromResult(Result.Success<IZafiroDirectory>(new SeaweedDirectory(path, seaweedFSClient)));
    }

    //public async Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path)
    //{
    //    var result = await Result
    //        .Try(() => seaweedFSClient.GetContents(path + "/"), exception => ExceptionHandler(exception, path + "/"))
    //        .Map(folder => folder.Entries?.Where(x => x.Chunks == null && x.Inode == 0) ?? Enumerable.Empty<DirectoryItem>())
    //        .Map(directoryItems => directoryItems.Select(item => (ZafiroPath) item.FullPath[1..]));

    //    return result;
    //}

    //public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path)
    //{
    //    return Result
    //        .Try(() => seaweedFSClient.GetContents(path), exception => ExceptionHandler(exception, path))
    //        .Map(folder => folder.Entries?.Where(x => x.Chunks != null) ?? Enumerable.Empty<DirectoryItem>())
    //        .Map(directoryItems => directoryItems.Select(item => (ZafiroPath) item.FullPath[1..]));
    //}

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return Task.FromResult(Result.Success<IZafiroFile>(new SeaweedFile(path, seaweedFSClient)));
    }

    //private async Task<Result<IZafiroFile>> GetFileCore(ZafiroPath path, Func<Task<Result>> checkIsValid)
    //{
    //    var r = await checkIsValid();

    //    return r.Map(() => (IZafiroFile) new SeaweedFile(path, this));
    //}

    //public Task<Result> SetContent(ZafiroPath path, Stream stream)
    //{
    //    return Result.Try(() => seaweedFSClient.Upload(path, stream));
    //}

    //public Task<Result<Stream>> GetContent(ZafiroPath path)
    //{
    //    return Result.Try(() => seaweedFSClient.GetFileContent(path));
    //}

    //private static string ExceptionHandler(Exception exception, string path)
    //{
    //    return exception is ApiException e ? e.StatusCode == HttpStatusCode.NotFound ? $"Path not found {path}" : exception.ToString() : exception.ToString();
    //}

    //public Task<Result<IZafiroFile>> GetFileWithoutChecks(ZafiroPath zafiroPath)
    //{
    //    return Task.FromResult(Result.Success((IZafiroFile)new SeaweedFile(zafiroPath, this)));
    //}

    //public Task<Result<IZafiroDirectory>> GetDirectoryWithoutChecks(ZafiroPath zafiroPath)
    //{
    //    return Task.FromResult(Result.Success((IZafiroDirectory)new SeaweedDirectory(zafiroPath, this)));
    //}
}