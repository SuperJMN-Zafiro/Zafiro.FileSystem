namespace Zafiro.FileSystem.NewComparer;

public record LeftOnlyDiff : FileDiff
{
    public IFile Left { get; }

    public LeftOnlyDiff(IFile left)
    {
        Left = left;
    }
}