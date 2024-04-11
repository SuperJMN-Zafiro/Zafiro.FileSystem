using Zafiro.FileSystem.Lightweight;
using Zafiro.Mixins;
using Directory = Zafiro.FileSystem.Lightweight.Directory;
using File = Zafiro.FileSystem.Lightweight.File;

namespace Zafiro.FileSystem.Tests;

public class LightweighTests
{
    [Fact]
    public async Task Refactor_this()
    {
        var subdirs = new List<IDirectory>
        {
            new Directory("Subdir",
            [
                new File("File3.txt", () => Task.FromResult(Result.Success("".ToStream()))),
                new File("File4.txt", () => Task.FromResult(Result.Success("".ToStream())))
            ], new List<IDirectory>())
        };
        var sut =
            new Directory("root", [
                    new File("File1.txt", () => Task.FromResult(Result.Success("".ToStream()))),
                    new File("File2.txt", () => Task.FromResult(Result.Success("".ToStream())))
                ], 
                subdirs);

        var allDirs = await sut.GetFilesInTree("root");
    }
}