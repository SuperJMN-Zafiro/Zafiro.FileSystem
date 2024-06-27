using DynamicData;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.VNext.Tests;

public class FileSystemRepositoryTests
{
    [Fact]
    public async Task DDTest()
    {
        var fs = new LocalFileSystem(new System.IO.Abstractions.FileSystem());
        var folder = fs.GetFolder("home/jmn/Escritorio");


        folder.Files.OnItemAdded(file => { }).Subscribe();
        
        var result = await folder.AddOrUpdateFile([new File("Pepito.txt", "Hehehe")]);
        await Task.Delay(10000);
        result.Should().Succeed();
    }
}