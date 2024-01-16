using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;
using Zafiro.Actions;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests;

public class ZafiroFileMixinTests
{
    [Fact]
    public async Task Timeout_should_be_mapped_as_result()
    {
        var source = new TestZafiroFile();
        var dest = new FileSystemRoot(new ObservableFileSystem(new WindowsZafiroFileSystem(new MockFileSystem()))).GetFile("Test.txt");
        var timeoutScheduler = new TestScheduler();
        timeoutScheduler.AdvanceBy(4);
        var result = await source.Copy(dest, Maybe<IObserver<LongProgress>>.None, readTimeout: TimeSpan.FromTicks(2));
        result.Should().Fail();
    }
}

public class TestZafiroFile : IZafiroFile
{
    public IObservable<byte> Contents => Observable.Never((byte)0);
    public Task<Result<bool>> Exists { get; }
    public ZafiroPath Path { get; }
    public Task<Result<FileProperties>> Properties => Task.FromResult(Result.Success(new FileProperties(false, DateTimeOffset.MinValue, 1000)));
    public Task<Result<IDictionary<HashMethod, byte[]>>> Hashes { get; }
    public IFileSystemRoot FileSystem { get; }
    public Task<Result> Delete() => throw new NotImplementedException();

    public Task<Result> SetContents(IObservable<byte> contents, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}