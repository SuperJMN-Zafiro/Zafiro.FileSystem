using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS.Tests;

public class Factory
{
    public static SeaweedFSClient CreateSut()
    {
        return new SeaweedFSClient(new HttpClient(){ BaseAddress = new Uri("http://192.168.1.29:8888")});
    }
}