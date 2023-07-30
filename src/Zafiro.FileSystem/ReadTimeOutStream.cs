﻿using System.Reactive.Linq;

namespace Zafiro.FileSystem;

public class ReadTimeOutStream : Stream
{
    private readonly Stream inner;

    public ReadTimeOutStream(Stream stream)
    {
        inner = stream;
    }

    public override void Flush()
    {
        inner.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return Observable.Start(() => inner.Read(buffer, offset, count)).Timeout(TimeSpan.FromMilliseconds(ReadTimeout)).Wait();
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

    public override int ReadTimeout { get; set; }

    public override bool CanTimeout => true;

    public override bool CanRead => inner.CanRead;

    public override bool CanSeek => inner.CanSeek;

    public override bool CanWrite => inner.CanWrite;

    public override long Length => inner.Length;

    public override long Position
    {
        get => inner.Position;
        set => inner.Position = value;
    }
}