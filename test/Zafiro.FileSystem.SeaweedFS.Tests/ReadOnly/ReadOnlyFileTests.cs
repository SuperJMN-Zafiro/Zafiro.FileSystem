using Xunit;

namespace Zafiro.FileSystem.SeaweedFS.Tests;

public class ReadOnlyFileTests
{
    [Theory]
    [InlineData("Juegos/ROMs/Gameboy/Atomic Punk (USA).zip")]
    [InlineData("file.txt")]
    public async Task Retrieving_directory_should_succeed(string path)
    {
        var seaweedFSClient = Factory.CreateSut();
        var result = await SeaweedFSFile.From(path, seaweedFSClient);
        result.Should().Succeed();
    }
    
    [Fact]
    public async Task Retrieving_inexistent_fails()
    {
        var seaweedFSClient = Factory.CreateSut();
        var result = await SeaweedFSFile.From("Inexistent", seaweedFSClient);
        result.Should().Fail();
    }
}