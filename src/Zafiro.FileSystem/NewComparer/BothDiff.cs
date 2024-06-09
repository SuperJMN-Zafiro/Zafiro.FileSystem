namespace Zafiro.FileSystem.NewComparer;

public record BothDiff : FileDiff
{
    public IRootedFile Left { get; }
    public IRootedFile Right { get; }

    public BothDiff(IRootedFile left, IRootedFile right)
    {
        Left = left;
        Right = right;
    }
}