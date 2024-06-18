using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Comparer;

public record LeftOnlyDiff : FileDiff
{
    public IRootedFile Left { get; }

    public LeftOnlyDiff(IRootedFile left)
    {
        Left = left;
    }
}