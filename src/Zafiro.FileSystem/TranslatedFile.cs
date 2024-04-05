using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class TranslatedFile : IZafiroFile
{
    public Func<ZafiroPath, ZafiroPath> TranslatePath { get; }
    public IZafiroFile File { get; }

    public TranslatedFile(IZafiroFile file, IZafiroDirectory root, Func<ZafiroPath, ZafiroPath> translatePath)
    {
        File = file;
        Root = root;
        TranslatePath = translatePath;
    }

    public IZafiroDirectory Root { get; }

    public IObservable<byte> Contents => File.Contents;

    public Task<Result<bool>> Exists => File.Exists;

    public ZafiroPath Path => TranslatePath(File.Path);

    public Task<Result<FileProperties>> Properties => File.Properties;

    public Task<Result<IDictionary<HashMethod, byte[]>>> Hashes => File.Hashes;

    public IFileSystemRoot FileSystem => File.FileSystem;

    public Task<Result> Delete() => File.Delete();

    public Task<Result> SetContents(IObservable<byte> contents, CancellationToken cancellationToken = default) => File.SetContents(contents, cancellationToken);

    public Task<Result<Stream>> GetData() => File.GetData();

    public Task<Result> SetData(Stream stream, CancellationToken cancellationToken = default) => File.SetData(stream, cancellationToken);

    public override string ToString() => $"{Path} ({File.ToString()})";
}