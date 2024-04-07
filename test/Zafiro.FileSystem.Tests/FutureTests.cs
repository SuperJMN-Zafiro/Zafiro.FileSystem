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
        var subdirs = new List<DataNode>
        {
            new("Subdir", new InMemoryDataTree(
            [
                new DataEntry("File3.txt", new InMemoryData(() => Task.FromResult(Result.Success("".ToStream())))),
                new DataEntry("File4.txt", new InMemoryData(() => Task.FromResult(Result.Success("".ToStream()))))
            ], new List<DataNode>()))
        };
        var sut = new InMemoryDataTree(
            [
                new DataEntry("File1.txt", new InMemoryData(() => Task.FromResult(Result.Success("".ToStream())))),
                new DataEntry("File2.txt", new InMemoryData(() => Task.FromResult(Result.Success("".ToStream()))))
            ]
            , subdirs);

        var allDirs = await sut.GetAllEntries("root");
    }
}