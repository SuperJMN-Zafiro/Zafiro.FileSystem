namespace Zafiro.FileSystem.NewComparer;

public record RightOnlyDiff : FileDiff
{
    public IFile Right { get; }

    public RightOnlyDiff(IFile right)
    {
        Right = right;
    }
}