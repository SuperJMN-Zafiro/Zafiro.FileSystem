using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Xunit;
using Zafiro.FileSystem.Comparer;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests;

public class RootedDirectoryTests
{
    [Fact]
    public async Task RootedDir()
    {
        var mockFileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>()
            {
                ["c:\\Dir\\File1"] = new("Test"),
                ["c:\\Dir\\File2"] = new("Test"),
                ["c:\\Dir\\Level1\\File3"] = new("Test"),
                ["c:\\Dir\\Level1\\Level2\\File4"] = new("Test"),
            });

        var windowsZafiroFileSystem = new WindowsZafiroFileSystem(mockFileSystem);
        var fs = new FileSystemRoot(windowsZafiroFileSystem);
        var dir = fs.GetDirectory("c:/Dir").RelativeToItself();
        var result = await dir.GetFilesInTree().MapMany(file => file.Path.ToString());
        result.Should().Succeed().And.Subject.Value.Should().BeEquivalentTo(["File1", "File2", "Level1/File3", "Level1/Level2/File4"]);
    }

    [Fact]
    public async Task Mount()
    {
        var mockFileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>()
            {
                ["c:\\Application\\File1"] = new("Test"),
                ["c:\\Application\\File2"] = new("Test"),
                ["c:\\Output"] = new MockDirectoryData(),
            });


        var fs = new FileSystemRoot(new WindowsZafiroFileSystem(mockFileSystem));
        var mounted = fs.GetDirectory("c:/Application").MountIn(fs.GetDirectory("c:/Output"));
        var result = await mounted.GetFilesInTree().MapMany(file => file.Path.ToString());
        result.Should().Succeed().And.Subject.Value.Should().BeEquivalentTo(["c:/Output/File1", "c:/Output/File2"]);
    }
}