using CSharpFunctionalExtensions;
using Xunit;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.FileSystem.VNext.Tests;

namespace Zafiro.FileSystem.SeaweedFS.Tests;

public class Class1
{
    [Fact]
    public async Task Test()
    {
        var seaweedFSClient = new SeaweedFSClient(new HttpClient() { BaseAddress = new Uri("http://192.168.1.29:8888") });
        var sut = await SeaweedFSDirectory
                .From("Juegos/ROMs", seaweedFSClient)
            .Bind(dir => dir.ToDirectory());

        var result = await SeaweedFSFile.From("Juegos/ROMs/3DS/Mario Kart 7.3ds", seaweedFSClient);
        
        sut.Should().Succeed();
    }
}