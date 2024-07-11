using CSharpFunctionalExtensions;
using Xunit;
using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.SeaweedFS.Tests;

public class ReadOnlyDirectoryTests
{
    [Theory]
    [InlineData("Juegos/ROMs")]
    [InlineData("Juegos")]
    public async Task From_should_succeed(string path)
    {
        var seaweedFSClient = Factory.CreateSut();
        var result = await SeaweedFSDirectory.From(path, seaweedFSClient);
        result.Should().Succeed();
    }
    
    [Theory]
    [InlineData("Juegos/ROMs")]
    [InlineData("Juegos")]
    public async Task Children_should_succeed(string path)
    {
        var seaweedFSClient = Factory.CreateSut();
        var result = await SeaweedFSDirectory.From(path, seaweedFSClient).Bind(x => x.Children());
        result.Should().Succeed().And.Subject.Value.Should().NotBeEmpty();
    }
}