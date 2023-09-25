// See https://aka.ms/new-console-template for more information

using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Local;

await CopyDir();
//await CopyFile();

Console.ReadLine();

async Task CopyFile()
{
    var fs = new LocalFileSystem(logger: Maybe<ILogger>.None);
    var action = from src in fs.GetFile("D:\\5 - Unimportant\\Temp\\One\\VMware-workstation-full-16.2.0-18760230.exe")
        from dst in fs.GetFile("D:\\5 - Unimportant\\Temp\\Two\\VMware-workstation-full-16.2.0-18760230.exe")
        from c in CopyFileAction.Create(src, dst)
        select c;

    var result = await action.Bind(async directoryAction =>
    {
        directoryAction.Progress.Subscribe(progress => Console.WriteLine(progress.Value));
        var execute = await directoryAction.Execute(CancellationToken.None);
        return execute;
    });
}

async Task CopyDir()
{
    var fs = new LocalFileSystem(logger: Maybe<ILogger>.None);
    var action = from src in fs.GetDirectory("D:\\5 - Unimportant\\Temp\\One")
        from dst in fs.GetDirectory("D:\\5 - Unimportant\\Temp\\Two")
        from c in CopyDirectoryAction.Create(src, dst)
        select c;

    var result = await action.Bind(async directoryAction =>
    {
        directoryAction.Progress.Subscribe(progress => Console.WriteLine(progress.Value));
        var execute = await directoryAction.Execute(CancellationToken.None);
        return execute;
    });
}