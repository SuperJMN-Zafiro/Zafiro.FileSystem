namespace Zafiro.FileSystem.Lightweight;

public interface ISlimDirectory : INode
{
    public IEnumerable<INode> Children { get; }
}