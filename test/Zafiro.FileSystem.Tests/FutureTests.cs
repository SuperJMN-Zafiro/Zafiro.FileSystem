using ClassLibrary1;
using CSharpFunctionalExtensions;
using Xunit;

namespace Zafiro.FileSystem.Tests;

public class FutureTests
{
    [Fact]
    public async Task Custom()
    {
        var root = new InMemoryDirectory("root", Maybe<IDirectory>.None, parent => new List<IFile>
        {
            new InMemoryFile("name", Maybe<IDirectory>.From(parent)),
        }, directory => new List<IDirectory>()
        {
            new InMemoryDirectory("Hola", Maybe<IDirectory>.From(directory), directory1 => Enumerable.Empty<IFile>(), d => Enumerable.Empty<IDirectory>())
        });

        var files = await root.GetFiles();
        var f = files.Value.First();
        var path = await f.GetPath();
    }
}