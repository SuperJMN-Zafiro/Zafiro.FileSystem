namespace Zafiro.FileSystem;

using System;
using System.IO;
using System.Reactive.Subjects;

public class PositionReportingStream : Stream, IObservable<long>
{
    private readonly Stream source;
    private readonly BehaviorSubject<long> positionSubject;

    public PositionReportingStream(Stream source)
    {
        this.source = source;
        this.positionSubject = new BehaviorSubject<long>(source.Position);
    }

    public IDisposable Subscribe(IObserver<long> observer) => positionSubject.Subscribe(observer);

    public override void Flush()
    {
        source.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int result = source.Read(buffer, offset, count);
        positionSubject.OnNext(source.Position);
        return result;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        long result = source.Seek(offset, origin);
        positionSubject.OnNext(source.Position);
        return result;
    }

    public override void SetLength(long value)
    {
        source.SetLength(value);
        positionSubject.OnNext(source.Position);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        source.Write(buffer, offset, count);
        positionSubject.OnNext(source.Position);
    }

    public override bool CanRead => source.CanRead;

    public override bool CanSeek => source.CanSeek;

    public override bool CanWrite => source.CanWrite;

    public override long Length => source.Length;

    public override long Position
    {
        get => source.Position;
        set
        {
            source.Position = value;
            positionSubject.OnNext(value);
        }
    }
}
