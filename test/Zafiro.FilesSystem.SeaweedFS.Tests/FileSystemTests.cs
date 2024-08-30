using CSharpFunctionalExtensions;
using FluentAssertions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FilesSystem.SeaweedFS.Tests;

public class FileSystemTests
{
    [Fact]
    public async Task GetDirectory()
    {
        var fs = new FileSystem.SeaweedFS.FileSystem(new SeaweedFSClient(new HttpClient(){ BaseAddress = new Uri("http://192.168.1.29:8888")}));
        var result = await fs.GetDirectory("eBooks");
        result.Should().Succeed();
    }
    
    [Fact]
    public async Task GetContents()
    {
        var fs = new FileSystem.SeaweedFS.FileSystem(new SeaweedFSClient(new HttpClient(){ BaseAddress = new Uri("http://192.168.1.29:8888")}));
        var result = await fs.GetDirectory("Juegos").Map(x => x.ToDirectory());
        result.Should().Succeed();
    }
}