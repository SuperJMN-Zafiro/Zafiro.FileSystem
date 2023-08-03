using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Sftp;
using Zafiro.IO;

namespace Zafiro.FileSystem.Tests;

public class IntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var sftp = await SftpFileSystem.Create("192.168.1.29", 22, "jmn", "fillme");
        var result = await sftp.Map(system => system.GetFile("/home/jmn/dotnet-install.sh"));
        var file = await result.Map(fs => fs.GetContents()).Map(stream => stream.ReadToEnd());

        var dir = await sftp.Map(system => system.GetDirectory("/home/jmn/seaweedfs-csi-driver"))
            .Map(directory => directory.GetFilesInTree());
    }
}