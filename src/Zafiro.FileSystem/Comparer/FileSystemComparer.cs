using CSharpFunctionalExtensions;
using MoreLinq.Extensions;

namespace Zafiro.FileSystem.Comparer;

public class FileSystemComparer
{
    public Task<Result<IEnumerable<FileDiff>>> Diff(IZafiroDirectory origin, IZafiroDirectory destination)
    {
        var sourceFiles = FilesWithMetadata(origin).MapMany(fwm => (Key: GetKey(origin, fwm), File: fwm));
        var destinationFiles = FilesWithMetadata(destination).MapMany(fwm => (Key: GetKey(destination, fwm), File: fwm));;

        var diff = from s in sourceFiles from d in destinationFiles select GetDiffs(s, d);
        return diff;
    }

    private IEnumerable<FileDiff> GetDiffs(IEnumerable<(ZafiroPath Key, IZafiroFile File)> originFiles, IEnumerable<(ZafiroPath Key, IZafiroFile File)> destinationFiles)
    {
        return originFiles.FullJoin(destinationFiles,
            f => f.Key,
            left => (FileDiff) new LeftOnlyDiff(left.File),
            right => new RightOnlyDiff(right.File),
            (left, right) => new BothDiff(left.File, right.File));
    }

    private static ZafiroPath GetKey(IZafiroDirectory origin, IZafiroFile f)
    {
        return f.Path.MakeRelativeTo(origin.Path);
    }

    private static Task<Result<IEnumerable<IZafiroFile>>> FilesWithMetadata(IZafiroDirectory origin)
    {
        var filesInTree = origin.GetFilesInTree();
        return filesInTree;
    }
}