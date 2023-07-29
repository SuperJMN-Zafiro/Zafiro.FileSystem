using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Local;

public class LocalFile : IZafiroFile
{
    private readonly FileSystemInfo info;

    public LocalFile(FileSystemInfo info)
    {
        this.info = info;
    }

    public ZafiroPath Path => info.FullName.ToZafiroPath();

    public Task<Result<Stream>> GetContents()
    {
        return Task.FromResult(Result.Try(() => (Stream) File.OpenRead(Path)));
    }

    public Task<Result> SetContents(Stream stream)
    {
        return Result.Try(async () =>
        {
            var fileStream = File.OpenWrite(Path);
            await stream.CopyToAsync(fileStream);
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
}