namespace Zafiro.FileSystem.Local;

public class LocalFileSystem : IFileSystem
{
    public Task<IZafiroDirectory> GetDirectory(ZafiroPath path)
    {
        return Task.FromResult((IZafiroDirectory)new LocalDirectory(new DirectoryInfo(path)));
    }

    public Task<IZafiroFile> GetFile(ZafiroPath path)
    {
        return Task.FromResult((IZafiroFile)new LocalFile(new FileInfo(path)));
    }
}