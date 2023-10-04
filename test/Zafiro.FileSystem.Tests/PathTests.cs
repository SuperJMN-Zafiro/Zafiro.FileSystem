using FluentAssertions;

namespace Zafiro.FileSystem.Tests;

public class PathTests
{
    [Fact]
    public void Test1()
    {
        var path = new ZafiroPath("/");
        var combined = path.Combine("Pepito");

        combined.Should().Be(new ZafiroPath("/Pepito"));
    }

    [Fact]
    public void EmptyPath()
    {
        var path = new ZafiroPath("/");
        path.Path.Should().Be("/");
    }
}