using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Local;
using Zafiro.FileSystem.Unix;

namespace Zafiro.FileSystem.VNext.Tests;

public class DirectoryTests
{
    [Fact]
    public async Task Test()
    {
        var sut = new UnixDir("Pepito", new UnixNode[]
        {
            new UnixDir("Hi"), 
            new UnixFile("Filecito")
        });

        var root = new UnixRoot();
    }

    [Fact]
    public async Task TestDir()
    {
        var maybeDir = await DotNetDirectory.From("C:/Users/JMN/Desktop", new System.IO.Abstractions.FileSystem());
        var dir = await maybeDir
            .Bind(d => d.ToDirectory());
    }
}