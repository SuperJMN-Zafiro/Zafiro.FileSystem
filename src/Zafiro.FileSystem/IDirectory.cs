namespace Zafiro.FileSystem;

public interface IDirectory : INode
{
    public IEnumerable<INode> Children { get; }
}