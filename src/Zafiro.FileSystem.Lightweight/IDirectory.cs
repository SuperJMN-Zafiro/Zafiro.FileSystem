using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public interface IDirectory : INode
{
    public Task<Result<IEnumerable<INode>>> Children();
}

public interface ISlimDirectory : INode
{
    public IEnumerable<INode> Children { get; }
}

public class SlimDirectory : ISlimDirectory
{
    public SlimDirectory(string name, IEnumerable<INode> children)
    {
        Name = name;
        Children = children;
    }

    public string Name { get; }
    public IEnumerable<INode> Children { get; }
}