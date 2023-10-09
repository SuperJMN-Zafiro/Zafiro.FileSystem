using CSharpFunctionalExtensions;
using MoreLinq.Extensions;

namespace Zafiro.FileSystem.Comparer;

public class FileSystemComparer
{
    public async Task<Result<IEnumerable<FileDiff>>> Diff(IZafiroDirectory origin, IZafiroDirectory destination)
    {
        var originFiles = (await origin.GetFilesInTree().ConfigureAwait(false)).Map(files => files.Select(file => (file, GetKey(origin, file))));
        var destinationFiles = (await destination.GetFilesInTree().ConfigureAwait(false)).Map(files => files.Select(file => (file, GetKey(destination, file))));

        return from n in originFiles from q in destinationFiles select Join(n, q);
    }

    private IEnumerable<FileDiff> Join(IEnumerable<(IZafiroFile, ZafiroPath)> originFiles, IEnumerable<(IZafiroFile, ZafiroPath)> destinationFiles)
    {
        return originFiles.FullJoin(destinationFiles,
            f => f.Item2,
            left => (FileDiff)new LeftOnly(left.Item1),
            right => new RightOnly(right.Item1),
            (left, _) => new Both(left.Item1, left.Item2));
    }

    private static ZafiroPath GetKey(IZafiroDirectory origin, IZafiroFile f)
    {
        return f.Path.MakeRelativeTo(origin.Path);
    }
}