using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Linq;
using System.Text;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests.Local;

public class FileSystemTests
{
    [Fact]
    public async Task Create_file()
    {
        var mockFileSystem = new MockFileSystem();
        var sut = new LocalFileSystem2(mockFileSystem);
        var result = await sut.CreateFile("Pepito.txt");
        result.Should().Succeed();
        mockFileSystem.GetFile("Pepito.txt").Contents.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_file()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["Pepito.txt"] = new("Salute")
        });
        var sut = new LocalFileSystem2(mockFileSystem);
        var result = await sut.GetFileContents("Pepito.txt");

        var bytes = result.Should().Succeed().And.Subject.Value.ToList().GetAwaiter().GetResult().ToArray();
        Encoding.UTF8.GetString(bytes).Should().Be("Salute");
    }

    [Fact]
    public async Task Set_file_contents()
    {
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["Pepito.txt"] = new("Old content")
        });

        //var fs = new System.IO.Abstractions.FileSystem();
        //using (fs.File.Create("Pepito.txt")){ }

        var sut = new LocalFileSystem2(fs);
        IObservable<byte> toWrite = "Salute"u8.ToArray().ToObservable();
        var result = await sut.SetFileContents("Pepito.txt", toWrite);

        result.Should().Succeed();
        fs.GetFile("Pepito.txt").TextContents.Should().Be("Salute");
    }
}