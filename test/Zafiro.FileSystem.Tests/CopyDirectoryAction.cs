using CSharpFunctionalExtensions;
using FluentAssertions.CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests;

public class CopyDirectoryTests
{
    [Fact]
    public async Task TestSubdir()
    {
        var fs = new LocalFileSystem(logger: Maybe<ILogger>.None);
        var dir = await fs.GetDirectory("C:\\Users\\JMN\\Desktop")
            .Bind(directory => directory.DescendantDirectory("Wild")).ConfigureAwait(false);
    }


    [Fact]
    public async Task Test()
    {
        var fs = new LocalFileSystem(logger: Maybe<ILogger>.None);
        var action = from src in fs.GetDirectory("D:\\5 - Unimportant\\Temp\\One")
            from dst in fs.GetDirectory("D:\\5 - Unimportant\\Temp\\Two")
            from ca in CopyDirectoryAction.Create(src, dst)
            select ca;

        var result = await action.Bind(async directoryAction =>
        {
            directoryAction.Progress.Subscribe(progress => { });
            var execute = await directoryAction.Execute(CancellationToken.None).ConfigureAwait(false);
            return execute;
        }).ConfigureAwait(false);
        result.Should().BeSuccess();
    }
}

