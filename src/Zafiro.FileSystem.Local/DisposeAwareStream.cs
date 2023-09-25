using CSharpFunctionalExtensions;
using Serilog;
using Serilog.Core;

namespace Zafiro.FileSystem.Local;

public class DisposeAwareStream : Stream
{
    public string Name { get; }
    private readonly Stream inner;
    private readonly Maybe<ILogger> logger;

    public DisposeAwareStream(string name, Stream inner, Maybe<ILogger> logger)
    {
        Name = name;
        this.inner = inner;
        this.logger = logger;
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

    // ReSharper disable once RedundantOverriddenMember
    protected override void Dispose(bool disposing)
    {
        logger.Execute(l => l.Debug("Disposing stream {Name}", Name));
        inner.Dispose();
        base.Dispose(disposing);
        logger.Execute(l => l.Debug("Disposed stream {Name}", Name));
    }

    // ReSharper disable once RedundantOverriddenMember
    public override async ValueTask DisposeAsync()
    {
        logger.Execute(l => l.Debug("Disposing stream {Name}", Name));
        await inner.DisposeAsync();
        await base.DisposeAsync();
        logger.Execute(l => l.Debug("Disposed stream {Name}", Name));
    }
}