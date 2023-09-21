// See https://aka.ms/new-console-template for more information

using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Local;

var fs = new LocalFileSystem(logger: Maybe<ILogger>.None);
var action = from src in fs.GetDirectory("D:\\5 - Unimportant\\Temp\\One")
    from dst in fs.GetDirectory("D:\\5 - Unimportant\\Temp\\Two")
    select new CopyDirectoryAction(src, dst);

var result = await action.Bind(async directoryAction =>
{
    directoryAction.Progress.Subscribe(progress => Console.WriteLine(progress.Value));
    var execute = await directoryAction.Execute(CancellationToken.None);
    return execute;
});