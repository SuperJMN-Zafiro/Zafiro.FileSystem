namespace Zafiro.FileSystem.Tests;

public class NeverEndingStream : Stream
{
    public override bool CanTimeout => true;
    public override int ReadTimeout { get; set; }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new TimeoutException();
        return 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
    {
        await Task.Delay(10000);
        return await ReadAsync(buffer, cancellationToken);
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => long.MaxValue;
    public override long Position { get; set; }
}