using CSharpFunctionalExtensions;
using MoreLinq;

namespace Zafiro.FileSystem;

public class FileSystemComparer : IFileSystemComparer
{
    public async Task<Result<IEnumerable<Diff>>> Diff(IZafiroDirectory origin, IZafiroDirectory destination)
    {
        var originFiles = (await origin.GetFilesInTree().ConfigureAwait(false)).Map(files => files.Select(file => GetKey(origin, file)));
        var destinationFiles = (await destination.GetFilesInTree().ConfigureAwait(false)).Map(files => files.Select(file => GetKey(destination, file)));

        return from n in originFiles from q in destinationFiles select Join(n, q);
    }

    private IEnumerable<Diff> Join(IEnumerable<ZafiroPath> originFiles, IEnumerable<ZafiroPath> destinationFiles)
    {
        return originFiles.FullJoin(destinationFiles,
            f => f,
            left => new Diff(left, FileDiffStatus.LeftOnly),
            right => new Diff(right, FileDiffStatus.RightOnly),
            (left, _) => new Diff(left, FileDiffStatus.Both));
    }

    private static ZafiroPath GetKey(IZafiroDirectory origin, IZafiroFile f)
    {
        return f.Path.MakeRelativeTo(origin.Path);
    }
}