using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class RootedFile : IRootedFile
{
    public ZafiroPath Path { get; }

    public IFile Rooted => File;

    public IFile File { get; }

    public RootedFile(ZafiroPath path, IFile file)
    {
        Path = path;
        File = file;
    }

    public override string ToString() => this.FullPath();
    public Task<Result<Stream>> Open() => File.Open();
    public string Name => File.Name;
}