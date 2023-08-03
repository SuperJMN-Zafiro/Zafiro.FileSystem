using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Tests;

public class TestFile : IZafiroFile
{
    public ZafiroPath Path => "/home/Sample.txt";

    public Task<long> Size()
    {
        throw new NotImplementedException();
    }

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