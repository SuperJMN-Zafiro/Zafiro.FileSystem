using Zafiro.FileSystem.Dynamic;
using Zafiro.FileSystem.Local.Mutable;

namespace Zafiro.FileSystem.Local.Tests;

public class MutableFileTests
{
    [Fact]
    public async Task Test()
    {
        var fs = new System.IO.Abstractions.FileSystem();
        var directoryInfo = fs.DirectoryInfo.New("/home/jmn/Escritorio");
        var directory = new DotNetDirectory(directoryInfo);
        var mutableDir = new DotNetMutableDirectory(directory);
        var result = await mutableDir.AddOrUpdate(new File("Hola.txt", "SDD"));
        result.Should().Succeed();
    }

    [Fact]
    public async Task WatchHierarchy()
    {
        var fs = new System.IO.Abstractions.FileSystem();
        var directoryInfo = fs.DirectoryInfo.New("/home/jmn/Escritorio");
        var directory = new LocalDynamicDirectory(directoryInfo);
        directory.AllFiles().Subscribe(set => { });
        await Task.Delay(20000);
    }
}