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
        var dir = fs.GetDirectory("c:/Dir").AsRooted();
        var result = await dir.GetFilesInTree().Map(file => file.Path.ToString());
        result.Should().Succeed().And.Subject.Value.Should().BeEquivalentTo(["File1", "File2", "Level1/File3", "Level1/Level2/File4"]);
    }
}