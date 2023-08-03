using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Local;

public class LocalFile : IZafiroFile
{
    private readonly FileInfo info;

    public LocalFile(FileInfo info)
    {
        this.info = info;
    }

    public ZafiroPath Path => info.FullName.ToZafiroPath();

    public Task<long> Size() => Task.FromResult(info.Length);

    public Task<Result<Stream>> GetContents()
    {
        return Task.FromResult(Result.Try(() =>
        {
            EnsureFileExists(Path.FromZafiroPath());
            return (Stream)new DisposeAwareStream(Path, File.OpenRead(Path));
        }));
    }

    public Task<Result> SetContents(Stream stream)
    {
        return Result.Try(async () =>
        {
            EnsureFileExists(Path.FromZafiroPath());
            var fileStream = new DisposeAwareStream(Path, File.OpenWrite(Path));
            {
                await stream.CopyToAsync(fileStream);
            }
            await fileStream.DisposeAsync();
        });
    }

    public Task<Result> Delete()
    {
        return Task.FromResult(Result.Try(() => info.Delete()));
    }

    public override string ToString()
    {
        return Path;
    }

    private void EnsureFileExists(string path)
    {
        var directoryName = System.IO.Path.GetDirectoryName(path);

        if (directoryName != null && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        if (!File.Exists(path))
        {
            using (File.Create(path)) { }
        }
    }
}