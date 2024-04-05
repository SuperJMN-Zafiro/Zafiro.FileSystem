using ClassLibrary1;
using CSharpFunctionalExtensions;
using Xunit;

namespace Zafiro.FileSystem.Tests;

public class FutureTests
{
    [Fact]
    public async Task Custom()
    {
        var root = new InMemoryDirectory("Root", Maybe<IDirectory>.None, parent => new List<IFile>
        {
            new InMemoryFile("File2.txt", Maybe<IDirectory>.From(parent)),
        }, directory =>
        [
            new InMemoryDirectory("Subdir", Maybe<IDirectory>.From(directory), parent =>
            [
                new InMemoryFile("File2.txt", Maybe<IDirectory>.From(parent))
            ], _ => Enumerable.Empty<IDirectory>())
        ]);

        var files = await root.GetFiles();
        var f = files.Value.First();
        var path = await f.GetPath();
    }
}