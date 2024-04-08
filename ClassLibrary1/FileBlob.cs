using System.Diagnostics;
using System.IO.Abstractions;
using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public class FileBlob(IFileInfo file): IBlob
{
    public Func<Task<Result<Stream>>> StreamFactory => () => Task.FromResult(Result.Try(() =>
    {
        if (Name == "AvaloniaSyncer.Desktop")
        {
            using var fileSystemStream = file.OpenRead();
            using (var output = File.OpenWrite("c:\\users\\jmn\\desktop\\myexe.txt"))
            {
                fileSystemStream.CopyTo(output);
            }

            Debugger.Break();
        }
        return (Stream) file.OpenRead();
    }));
    public string Name => file.Name;
}