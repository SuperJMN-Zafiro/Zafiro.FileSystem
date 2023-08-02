using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Local;

public class LocalFileSystem : IFileSystem
{
    public Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroDirectory>(() => new LocalDirectory(new DirectoryInfo(path))));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return Task.FromResult(Result.Try<IZafiroFile>(() => new LocalFile(new FileInfo(path))));
    }
}