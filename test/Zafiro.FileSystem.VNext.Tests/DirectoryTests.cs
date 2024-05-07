using System.Transactions;
using Zafiro.FileSystem.Unix;

namespace Zafiro.FileSystem.VNext.Tests;

public class DirectoryTests
{
    [Fact]
    public async Task Test()
    {
        var sut = new UnixDir("Pepito", new UnixNode[]
        {
            new UnixDir("Hi"), 
            new UnixFile("Filecito")
        });

        var root = new UnixRoot();
    }
}