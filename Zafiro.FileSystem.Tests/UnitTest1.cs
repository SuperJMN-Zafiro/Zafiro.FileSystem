using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.FileSystem.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var sut = new Syncer(Maybe<ILogger>.None);
            sut.ReadTimeout = TimeSpan.FromSeconds(1);
            IZafiroDirectory source = new TestSourceDirectory();
            IZafiroDirectory destination = new TestDestinationDirectory();
            var diffs = new[] { new Diff("/Sample.txt", FileDiffStatus.Both) };
            var result = await sut.Sync(source, destination, diffs);
        }
    }

    public class TestDestinationDirectory : IZafiroDirectory
    {
        public ZafiroPath Path => "/home";
        public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
        {
            return Task.FromResult(Result.Success(Enumerable.Empty<IZafiroDirectory>()));
        }

        public Task<Result<IEnumerable<IZafiroFile>>> GetFiles()
        {
            return Task.FromResult(Result.Success(Enumerable.Empty<IZafiroFile>()));
        }

        public Task<Result<IZafiroFile>> GetFile(ZafiroPath destPath)
        {
            return Task.FromResult(Result.Success((IZafiroFile) new TestFile()));
        }
    }

    public class TestSourceDirectory : IZafiroDirectory
    {
        public ZafiroPath Path => "/home";
        public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
        {
            return Task.FromResult(Result.Success(Enumerable.Empty<IZafiroDirectory>()));
        }

        public Task<Result<IEnumerable<IZafiroFile>>> GetFiles()
        {
            return Task.FromResult(Result.Success(new [] { (IZafiroFile)new TestFile() }.AsEnumerable()));
        }

        public Task<Result<IZafiroFile>> GetFile(ZafiroPath destPath)
        {
            return Task.FromResult(Result.Success((IZafiroFile)new TestFile()));
        }
    }

    public class TestFile : IZafiroFile
    {
        public ZafiroPath Path => "/home/Sample.txt";
        
        public Task<Result<Stream>> GetContents()
        {
            return Task.FromResult(Result.Success((Stream)new NeverEndingStream()));
        }

        public async Task<Result> SetContents(Stream stream)
        {
            return await Result.Try(async () =>
            {
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
            });
        }

        public Task<Result> Delete()
        {
            throw new NotImplementedException();
        }
    }

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
}