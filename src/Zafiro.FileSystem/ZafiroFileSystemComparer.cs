using CSharpFunctionalExtensions;
using MoreLinq;

namespace Zafiro.FileSystem;

public class ZafiroFileSystemComparer
{
    public async Task<Result<IEnumerable<ZafiroFileDiff>>> Diff(IZafiroDirectory origin, IZafiroDirectory destination)
    {
        var originFiles = (await origin.GetFilesInTree()).Map(files => files.Select(file => new KeyedFile
        {
            Key = GetKey(origin, file),
            File = file
        }).ToList());

        var destinationFiles = (await destination.GetFilesInTree()).Map(files => files.Select(file => new KeyedFile
        {
            Key = GetKey(destination, file),
            File = file
        }).ToList());

        return from n in originFiles from q in destinationFiles select Join(n, q);
    }

    private IEnumerable<ZafiroFileDiff> Join(IEnumerable<KeyedFile> originFiles, IEnumerable<KeyedFile> destinationFiles)
    {
        return originFiles.FullJoin(destinationFiles,
            f => f.Key,
            left => new ZafiroFileDiff(Maybe.From(left.File), Maybe<IZafiroFile>.None),
            right => new ZafiroFileDiff(Maybe<IZafiroFile>.None, Maybe.From(right.File)),
            (left, right) => new ZafiroFileDiff(Maybe.From(left.File), Maybe.From(right.File)));
    }

    private static ZafiroPath GetKey(IZafiroDirectory origin, IZafiroFile f)
    {
        return f.Path.MakeRelativeTo(origin.Path);
    }

    private class KeyedFile
    {
        public ZafiroPath Key { get; set; }
        public IZafiroFile File { get; set; }
    }
}