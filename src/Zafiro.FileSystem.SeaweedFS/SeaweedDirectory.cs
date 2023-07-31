using System.Net;
using CSharpFunctionalExtensions;
using Refit;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Directory = Zafiro.FileSystem.SeaweedFS.Filer.Client.Directory;
using File = Zafiro.FileSystem.SeaweedFS.Filer.Client.File;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedDirectory : IZafiroDirectory
{
    private readonly SeaweedFSClient seaweedFS;

    public SeaweedDirectory(ZafiroPath path, SeaweedFSClient seaweedFS)
    {
        Path = path;
        this.seaweedFS = seaweedFS;
    }

    public ZafiroPath Path { get; }

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
    {
        return Result
            .Try(() => seaweedFS.CreateFolder(Path))
            .Bind(() => Result.Try(() => seaweedFS.GetContents(Path), ExceptionHandler))
            .Map(GetDirectories);
    }

    private IEnumerable<IZafiroDirectory> GetDirectories(RootDirectory folder)
    {
        return folder.Entries
            .OfType<Directory>()
            .Select(f => new SeaweedDirectory(f.FullPath[1..], seaweedFS));
    }

    private IEnumerable<IZafiroFile> GetFiles(RootDirectory folder)
    {
        return folder.Entries
            .OfType<File>()
            .Select(f => new SeaweedFile(f.FullPath[1..], seaweedFS));
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles()
    {
        return Result
            .Try(() => seaweedFS.CreateFolder(Path))
            .Bind(() => Result.Try(() => seaweedFS.GetContents(Path), ExceptionHandler))
            .Map(GetFiles);
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath destPath)
    {
        return Task.FromResult(Result.Success((IZafiroFile)new SeaweedFile(destPath, seaweedFS)));
    }

    private string ExceptionHandler(Exception exception)
    {
        if (exception is ApiException { StatusCode: HttpStatusCode.NotFound } )
        {
            return $"Path not found: {Path}";
        }

        return exception.ToString();
    }

    public override string ToString()
    {
        return Path;
    }
}