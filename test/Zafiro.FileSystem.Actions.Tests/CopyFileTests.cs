using System.Reactive.Subjects;
using FluentAssertions;
using Zafiro.Actions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Local;
using Zafiro.FileSystem.Local.Mutable;

namespace Zafiro.FileSystem.Actions.Tests;

public class CopyFileTests
{
    [Fact]
    public async Task CopyFileTask()
    {
        var fs = new System.IO.Abstractions.FileSystem();
        var dotNetMutableFile = new DotNetMutableFile(new DotNetFile(fs.FileInfo.New("/home/jmn/Escritorio/Pepito.txt")));

        var task = new CopyFileAction(new StringData("Hola"), dotNetMutableFile);
        var observer = new Subject<LongProgress>();
        task.Progress.Subscribe(observer);
        var result = await task.Execute();
        result.Should().Succeed();
    }
}