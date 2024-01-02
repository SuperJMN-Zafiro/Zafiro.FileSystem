namespace Zafiro.FileSystem.Comparer;

public record LeftOnlyDiff : FileDiff
{
    public IZafiroFile Left { get; }

    public LeftOnlyDiff(IZafiroFile left)
    {
        Left = left;
    }
}