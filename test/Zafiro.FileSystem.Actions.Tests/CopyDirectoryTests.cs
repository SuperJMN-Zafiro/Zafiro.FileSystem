using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Zafiro.Actions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Local;
using Zafiro.FileSystem.Local.Mutable;

namespace Zafiro.FileSystem.Actions.Tests;

public class CopyDirectoryTests
{
    [Fact]
    public async Task CopyFileTask()
    {
        var fs = new System.IO.Abstractions.FileSystem();
        var directoryInfo = fs.DirectoryInfo.New("/home/jmn/Escritorio");
        var dotNetMutableDirectory = new DotNetMutableDirectory(new DotNetDirectory(directoryInfo));
        
        var result = CopyDirectoryAction.Create(new Directory("hola", new List<INode>()
        {
            new File("Contenido2.txt", "Algo"),
            new File("Contenido1.txt", "Algo2")
        }), dotNetMutableDirectory);
        var other = await result.Bind(action =>
        {
            var observer = new Subject<LongProgress>();
            action.Progress.Subscribe(observer);
            return action.Execute(CancellationToken.None);
        });

        other.Should().Succeed();
    }
}