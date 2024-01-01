namespace Zafiro.FileSystem.Comparer;

public record BothDiff : FileDiff
{
    public IZafiroFile Left { get; }
    public IZafiroFile Right { get; }

    public BothDiff(IZafiroFile left, IZafiroFile right)
    {
        Left = left;
        Right = right;
    }
}