using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class RootedFile : IZafiroFile
{
    private readonly IZafiroDirectory root;
    private readonly IZafiroFile file;

    public RootedFile(IZafiroFile file, IZafiroDirectory root)
    {
        this.file = file;
        this.root = root;
    }

    public IObservable<byte> Contents => file.Contents;

    public Task<Result<bool>> Exists => file.Exists;

    public ZafiroPath Path => file.Path.MakeRelativeTo(root.Path);

    public Task<Result<FileProperties>> Properties => file.Properties;

    public Task<Result<IDictionary<HashMethod, byte[]>>> Hashes => file.Hashes;

    public IFileSystemRoot FileSystem => file.FileSystem;

    public Task<Result> Delete() => file.Delete();

    public Task<Result> SetContents(IObservable<byte> contents, CancellationToken cancellationToken = default) => file.SetContents(contents, cancellationToken);

    public Task<Result<Stream>> GetData() => file.GetData();

    public Task<Result> SetData(Stream stream, CancellationToken cancellationToken = default) => file.SetData(stream, cancellationToken);
}