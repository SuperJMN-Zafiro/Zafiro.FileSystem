using System.IO.Abstractions.TestingHelpers;
using DynamicData;
using Zafiro.FileSystem.DynamicData;
using Zafiro.FileSystem.Local.Mutable;
using Zafiro.Reactive;

namespace Zafiro.FileSystem.VNext.Tests;

public class FileSystemRepositoryTests
{
    [Fact]
    public async Task DDTest()
    {
        var fs = new LocalFileSystem(new System.IO.Abstractions.FileSystem());
        var folder = fs.GetFolder("home/jmn/Escritorio");


        folder.Files.OnItemAdded(file => { }).Subscribe();
        
        var result = await folder.AddOrUpdateFile(new[] { new File("Pepito.txt", "Hehehe") });
        await Task.Delay(10000);
        result.Should().Succeed();
    }
}