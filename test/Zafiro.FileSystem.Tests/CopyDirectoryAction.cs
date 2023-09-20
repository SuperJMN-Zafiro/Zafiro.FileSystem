using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using FluentAssertions.CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
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
            .Bind(directory => directory.DescendantDirectory("Wild"));
    }


    [Fact]
    public async Task Test()
    {
        var fs = new LocalFileSystem(logger: Maybe<ILogger>.None);
        var action = from src in fs.GetDirectory("D:\\5 - Unimportant\\Temp\\One")
            from dst in fs.GetDirectory("D:\\5 - Unimportant\\Temp\\Two")
            select new CopyDirectoryAction(src, dst);

        var result = await action.Bind(async directoryAction =>
        {
            directoryAction.Progress.Subscribe(progress => { });
            var execute = await directoryAction.Execute(CancellationToken.None);
            return execute;
        });
        result.Should().BeSuccess();
    }
}

