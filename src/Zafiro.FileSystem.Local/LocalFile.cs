using CSharpFunctionalExtensions;
using System.IO.Abstractions;

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

public class DisposeAwareStream : Stream
{
    public string Name { get; }
    private Stream inner;

    public DisposeAwareStream(string name, FileStream inner)
    {
        Name = name;
        this.inner = inner;
    }

    public override void Flush()
    {
        inner.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return inner.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return inner.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        inner.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        inner.Write(buffer, offset, count);
    }

    public override bool CanRead => inner.CanRead;

    public override bool CanSeek => inner.CanSeek;

    public override bool CanWrite => inner.CanWrite;

    public override long Length => inner.Length;

    public override long Position
    {
        get => inner.Position;
        set => inner.Position = value;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

    public override ValueTask DisposeAsync()
    {
        return base.DisposeAsync();
    }
}