using ClassLibrary1;
using CSharpFunctionalExtensions;
using Xunit;
using Zafiro.Mixins;

namespace Zafiro.FileSystem.Tests;

public class FutureTests
{
    [Fact]
    public async Task Custom()
    {
        var subdirs = new List<IBlobContainer>
        {
            new InMemoryBlobContainer("Subdir",
            [
                new InMemoryBlob("File3.txt", () => Task.FromResult(Result.Success("".ToStream()))),
                new InMemoryBlob("File4.txt", () => Task.FromResult(Result.Success("".ToStream())))
            ], new List<IBlobContainer>())
        };
        var sut = new InMemoryBlobContainer([
                new InMemoryBlob("File1.txt", () => Task.FromResult(Result.Success("".ToStream()))),
                new InMemoryBlob("File2.txt", () => Task.FromResult(Result.Success("".ToStream())))
            ]
            , subdirs);

        var allDirs = await sut.GetBlobsInTree("root");
    }
}