namespace Zafiro.FileSystem.NewComparer;

public record RightOnlyDiff : FileDiff
{
    public IRootedFile Right { get; }

    public RightOnlyDiff(IRootedFile right)
    {
        Right = right;
    }
}