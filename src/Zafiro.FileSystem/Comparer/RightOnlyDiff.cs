using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Comparer;

public record RightOnlyDiff : FileDiff
{
    public IRootedFile Right { get; }

    public RightOnlyDiff(IRootedFile right)
    {
        Right = right;
    }
}