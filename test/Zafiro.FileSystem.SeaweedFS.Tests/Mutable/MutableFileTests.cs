using Xunit;

namespace Zafiro.FileSystem.SeaweedFS.Tests.Mutable;

public class MutableFileTests
{
    [Fact]
    public async Task Inexistent_file_should_not_exist()
    {
        var sut = new SeaweedFS.Mutable.File("Inexistent.txt", Factory.CreateSut());
        var result = await sut.Exists();
        result.Should().SucceedWith(false);
    }
    
    [Fact]
    public async Task Existent_file_should_exist()
    {
        var sut = new SeaweedFS.Mutable.File("file.txt", Factory.CreateSut());
        var result = await sut.Exists();
        result.Should().SucceedWith(true);
    }
}