namespace Zafiro.FileSystem.Comparer;

public record RightOnlyDiff : FileDiff
{
    public IZafiroFile Right { get; }

    public RightOnlyDiff(IZafiroFile right)
    {
        Right = right;
    }
}