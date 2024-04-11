using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class File : IFile
{
    public File(string name, Func<Task<Result<Stream>>> streamFactory)
    {
        Open = streamFactory;
        Name = name;
    }

    public Func<Task<Result<Stream>>> Open { get; }

    public string Name { get; }
}