using Zafiro.FileSystem.Local;
using Zafiro.FileSystem.Unix;
using Zafiro.FileSystem.VNext.Interfaces;
using Directory = Zafiro.FileSystem.Lightweight.Directory;

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
        var maybeDir = await DotNetDirectory.From("C:/Users/JMN/Desktop");
        var dir = await maybeDir
            .Bind(d => d.ToLightweight());
    }
}

public static class Mixin
{
    public static Task<Result<IDirectory>> ToLightweight(this IAsyncDir asyncDir)
    {
        return asyncDir.Children()
            .Map(nodes => nodes.ToList())
            .Bind(async nodes =>
            {
                var dirs = nodes.OfType<IAsyncDir>();
                var files = nodes.OfType<IFile>().Cast<INode>();
                var dirResults = await Task.WhenAll(dirs.Select(ToLightweight));
                return dirResults.Combine().Map(d => d.Concat(files));
            })
            .Map(enumerable => (IDirectory)new Directory(asyncDir.Name, enumerable));
    }
}