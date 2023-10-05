using FluentAssertions.CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Tests;

public class PathTests
{
    [Fact]
    public void Valid_path()
    {
        ZafiroPath.Create("This is a path").Should().BeSuccess();
    }

    [Fact]
    public void Valid_path_with_subparts()
    {
        ZafiroPath.Create("This is a path/sub dir").Should().BeSuccess();
    }

    [Fact]
    public void Invalid_path()
    {
        ZafiroPath.Create("/").Should().BeFailure();
    }

    [Fact]
    public void Empty_path()
    {
        var path = ZafiroPath.Create("");
        path.Should().BeFailure();
    }

    [Fact]
    public void Empty_path_multiple_spaces()
    {
        var path = ZafiroPath.Create("   ");
        path.Should().BeFailure();
    }
}