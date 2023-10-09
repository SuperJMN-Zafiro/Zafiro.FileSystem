using System.IO.Abstractions.TestingHelpers;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests;

public class SyncDirectoriesActionTests
{
    [Fact]
    public async Task Sync()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["C:\\Source\\File1.txt"] = new("Hi"),
            ["C:\\Source\\File2.txt"] = new("How"),
            ["C:\\Destination\\File1.txt"] = new("Heya!")
        });

        var local = new LocalFileSystem(mockFileSystem, Maybe<ILogger>.None);
        var r = await local.GetDirectory("C:/Source").CombineAndBind(local.GetDirectory("C:/Destination"), (directory, zafiroDirectory) => CopyLeftFilesToRightSideAction.Create(directory, zafiroDirectory));
        var execution = await r.Bind(s => s.Execute(CancellationToken.None));
        execution.Should().BeSuccess();

        mockFileSystem.GetFile("C:\\Source\\File1.txt").TextContents.Should().Be("Hi");
        mockFileSystem.GetFile("C:\\Source\\File2.txt").TextContents.Should().Be("How");
        mockFileSystem.GetFile("C:\\Destination\\File1.txt").TextContents.Should().Be("Heya!");
        mockFileSystem.GetFile("C:\\Destination\\File2.txt").TextContents.Should().Be("How");
    }
}