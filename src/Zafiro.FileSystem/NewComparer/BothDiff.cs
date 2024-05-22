namespace Zafiro.FileSystem.NewComparer;

public record BothDiff : FileDiff
{
    public IFile Left { get; }
    public IFile Right { get; }

    public BothDiff(IFile left, IFile right)
    {
        Left = left;
        Right = right;
    }
}