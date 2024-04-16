using System.Text;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class File : IFile
{
    private readonly Func<Task<Result<Stream>>> streamFactory;

    public File(string name, string content) : this(name, () => Task.FromResult(Result.Success((Stream)new MemoryStream(Encoding.UTF8.GetBytes(content)))))
    {
    }
    
    public File(string name, Func<Task<Result<Stream>>> streamFactory)
    {
        this.streamFactory = streamFactory;
        Name = name;
    }

    public Task<Result<Stream>> Open() => streamFactory();

    public string Name { get; }
}