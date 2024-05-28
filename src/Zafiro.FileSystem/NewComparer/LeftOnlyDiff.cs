namespace Zafiro.FileSystem.NewComparer;

public record LeftOnlyDiff : FileDiff
{
    public IRootedFile Left { get; }

    public LeftOnlyDiff(IRootedFile left)
    {
        Left = left;
    }
}