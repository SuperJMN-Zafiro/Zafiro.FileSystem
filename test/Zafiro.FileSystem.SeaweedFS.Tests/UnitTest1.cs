using CSharpFunctionalExtensions;
using Serilog;
using Xunit;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://192.168.1.29:8888")
        };
        var fs = new SeaweedFileSystem(new SeaweedFSClient(httpClient), Maybe<ILogger>.None);
        var props = await fs.GetFileProperties("Documentos/ALIENZORRO.pdf");
        props.Should().Succeed().And.Subject.Should().Subject.Value.Checksum.Should().NotBeEmpty();
    }
}
