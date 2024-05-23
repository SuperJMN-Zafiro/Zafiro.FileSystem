using MoreLinq.Extensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Lightweight;
using Zafiro.FileSystem.NewComparer;

namespace AvaloniaSyncer.Console;

public static class FileListMixin
{
    public static IEnumerable<FileDiff> Diff(this IEnumerable<IRootedFile> leftFiles, IEnumerable<IRootedFile> rightFiles)
    {
        return leftFiles.FullJoin(rightFiles,
            f => f.FullPath(),
            left => (FileDiff) new LeftOnlyDiff(left),
            right => new RightOnlyDiff(right),
            (left, right) => new BothDiff(left, right));
    }
}