namespace Zafiro.FileSystem.Lightweight;

public interface IDirectory : INode
{
    public IEnumerable<INode> Children { get; }
}