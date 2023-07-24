using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Local;

public class LocalDirectory : IZafiroDirectory
{
    private readonly DirectoryInfo directoryInfo;

    public LocalDirectory(DirectoryInfo directoryInfo)
    {
        this.directoryInfo = directoryInfo;
    }

    public ZafiroPath Path => directoryInfo.FullName.ToZafiroPath();

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
    {
        return Task.FromResult(Result.Try(() => directoryInfo.GetDirectories().Select(info => (IZafiroDirectory) new LocalDirectory(info))));
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles()
    {
        return Task.FromResult(Result.Try(() => directoryInfo.GetFiles().Select(info => (IZafiroFile) new LocalFile(info))));
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath destPath)
    {
        return Task.FromResult(Result.Success((IZafiroFile)new LocalFile(new FileInfo(destPath))));
    }

    public override string ToString()
    {
        return Path;
    }
}