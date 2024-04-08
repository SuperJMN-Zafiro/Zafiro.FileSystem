using Zafiro.FileSystem.Lightweight;
using Zafiro.Mixins;

namespace Zafiro.FileSystem.Tests;

public class FutureTests
{
    [Fact]
    public async Task Custom()
    {
        var subdirs = new List<IBlobContainer>
        {
            new BlobContainer("Subdir",
            [
                new Blob("File3.txt", () => Task.FromResult(Result.Success("".ToStream()))),
                new Blob("File4.txt", () => Task.FromResult(Result.Success("".ToStream())))
            ], new List<IBlobContainer>())
        };
        var sut = 
                new BlobContainer([
                new Blob("File1.txt", () => Task.FromResult(Result.Success("".ToStream()))),
                new Blob("File2.txt", () => Task.FromResult(Result.Success("".ToStream())))
            ]
            , subdirs);

        var allDirs = await sut.GetBlobsInTree("root");
    }
}