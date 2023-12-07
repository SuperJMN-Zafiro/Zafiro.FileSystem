using CSharpFunctionalExtensions;
using MoreLinq.Extensions;

namespace Zafiro.FileSystem.Evolution.Comparer;

public class FileSystemComparer
{
    public async Task<Result<IEnumerable<FileDiff>>> Diff(IZafiroDirectory2 origin, IZafiroDirectory2 destination)
    {
        var originFiles = (await origin.GetFilesInTree().ConfigureAwait(false)).Map(files => files.Select(file => GetKey(origin, file)));
        var destinationFiles = (await destination.GetFilesInTree().ConfigureAwait(false)).Map(files => files.Select(file => GetKey(destination, file)));

        return from n in originFiles from q in destinationFiles select Join(n, q);
    }

    private IEnumerable<FileDiff> Join(IEnumerable<ZafiroPath> originFiles, IEnumerable<ZafiroPath> destinationFiles)
    {
        return originFiles.FullJoin(destinationFiles,
            f => f,
            left => (FileDiff)new LeftOnly(left),
            right => new RightOnly(right),
            (left, _) => new Both(left));
    }

    private static ZafiroPath GetKey(IZafiroDirectory2 origin, IZafiroFile2 f)
    {
        return f.Path.MakeRelativeTo(origin.Path);
    }
}