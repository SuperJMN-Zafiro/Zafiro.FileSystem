using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Local;

public class LocalFileSystem : IFileSystem
{
    public Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path)
    {
        return Task.FromResult(Result.Success<IZafiroDirectory>(new LocalDirectory(new DirectoryInfo(path))));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return Task.FromResult(Result.Success<IZafiroFile>(new LocalFile(new FileInfo(path))));
    }
}