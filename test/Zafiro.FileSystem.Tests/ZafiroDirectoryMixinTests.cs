using System.IO.Abstractions.TestingHelpers;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests;

public class ZafiroDirectoryMixinTests
{
    [Fact]
    public async Task Maybe_file()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["C:\\Dir1\\Dir2\\File.txt"] = new(""),
        });
        
        var fileSystem = new LocalFileSystem(mockFileSystem, Maybe<ILogger>.None);
        var result = await fileSystem.GetDirectory("C:/Dir1")
            .Bind(directory => directory.MaybeDescendantFile("Dir2/File.txt"));
        
        result.Should().BeSuccess().And.Subject.Value.Value.Should().BeAssignableTo<IZafiroFile>();
    }

    [Fact]
    public async Task Maybe_directory()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["C:\\Dir1\\Dir2\\File.txt"] = new(""),
        });

        var fileSystem = new LocalFileSystem(mockFileSystem, Maybe<ILogger>.None);
        var result = await fileSystem.GetDirectory("C:/Dir1")
            .Bind(directory => directory.MaybeDescendantDirectory("Dir2"));

        result.Should().BeSuccess().And.Subject.Value.Value.Should().BeAssignableTo<IZafiroDirectory>();
    }
}