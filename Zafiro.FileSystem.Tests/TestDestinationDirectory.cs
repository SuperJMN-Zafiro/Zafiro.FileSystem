using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Tests;

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