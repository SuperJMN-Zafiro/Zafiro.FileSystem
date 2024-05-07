using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public class RegularDirectory : IDirectory
{
    public RegularDirectory(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public Task<Result<IEnumerable<INode>>> Children() => throw new NotImplementedException();
}