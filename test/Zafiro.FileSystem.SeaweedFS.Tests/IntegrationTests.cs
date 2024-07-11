using CSharpFunctionalExtensions;
using Xunit;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS.Tests;

public class ClientTests
{
    [InlineData("Juegos/ROMs/Gameboy/Atomic Punk (USA).zip")]
    [InlineData("file.txt")]
    [Theory]
    public async Task GetFile(string path)
    {
        await AssertMetadata(path);
    }

    private static async Task AssertMetadata(string path)
    {
        var sut = new SeaweedFSClient(new HttpClient(){ BaseAddress = new Uri("http://192.168.1.29:8888")});
        var result = await sut.GetFileMetadata(path);
        result.Should().Succeed();
    }
}

public class IntegrationTests
{
    [Fact]
    public async Task Retrieve_directory()
    {
        var seaweedFSClient = new SeaweedFSClient(new HttpClient() { BaseAddress = new Uri("http://192.168.1.29:8888") });
        var sut = await SeaweedFSDirectory
                .From("Juegos/ROMs", seaweedFSClient)
            .Bind(dir => dir.ToDirectory());
        
        sut.Should().Succeed();
    }
}