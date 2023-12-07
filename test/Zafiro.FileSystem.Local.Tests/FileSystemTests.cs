using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Linq;
using System.Text;
using FluentAssertions.Extensions;
using Xunit;

namespace Zafiro.FileSystem.Local.Tests;

public class FileSystemTests
{
    [Fact]
    public async Task Create_file()
    {
        var mockFileSystem = new MockFileSystem();
        var sut = new LocalZafiroFileSystem(mockFileSystem);
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
        var sut = new LocalZafiroFileSystem(mockFileSystem);
        var result = await sut.Contents("Pepito.txt").ToList();

        Encoding.UTF8.GetString(result.ToArray()).Should().Be("Salute");
    }

    [Fact]
    public async Task Set_file_contents()
    {
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["Pepito.txt"] = new("Old content")
        });

        var sut = new LocalZafiroFileSystem(fs);
        IObservable<byte> toWrite = "Salute"u8.ToArray().ToObservable();
        var result = await sut.SetFileContents("Pepito.txt", toWrite);

        result.Should().Succeed();
        fs.GetFile("Pepito.txt").TextContents.Should().Be("Salute");
    }

    [Fact]
    public async Task Create_folder()
    {
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>());

        var sut = new LocalZafiroFileSystem(fs);
        var result = await sut.CreateDirectory("Folder");

        result.Should().Succeed();
        fs.Directory.Exists("Folder").Should().BeTrue();
    }

    [Fact]
    public async Task Get_file_properties()
    {
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["File.txt"] = new("Some content") { CreationTime = 30.January(2010)}
        });

        var sut = new LocalZafiroFileSystem(fs);
        var result = await sut.GetFileProperties("File.txt");

        result.Should().Succeed();
        result.Value.IsHidden.Should().Be(false);
        result.Value.Length.Should().Be(12L);
        result.Value.CreationTime.Should().Be(30.January(2010));
    }
}