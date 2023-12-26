using CSharpFunctionalExtensions;
using MoreLinq.Extensions;

namespace Zafiro.FileSystem.Comparer;

public static class Mixin
{
    public static Task<Result<IEnumerable<TResult>>> BindMany<TInput, TResult>(this Task<Result<IEnumerable<TInput>>> taskResult, Func<TInput, Result<TResult>> selector)
    {
        return taskResult.Bind(inputs => inputs.Select(selector).Combine());
    }

    public static Task<Result<IEnumerable<TResult>>> BindMany<TInput, TResult>(this Task<Result<IEnumerable<TInput>>> taskResult, Func<TInput, Task<Result<TResult>>> selector)
    {
        return taskResult.Bind(inputs => inputs.Select(selector).Combine());
    }
}

public class FileSystemComparer2
{
    public Task<Result<IEnumerable<FileDiff>>> Diff(IZafiroDirectory origin, IZafiroDirectory destination)
    {
        var sourceFiles = FilesWithMetadata(origin);
        var destinationFiles = FilesWithMetadata(origin);

        var diff = from s in sourceFiles from d in destinationFiles select GetDiffs(s, d);
        return diff;
    }

    private IEnumerable<FileDiff> GetDiffs(IEnumerable<FileWithMetadata> originFiles, IEnumerable<FileWithMetadata> destinationFiles)
    {
        return originFiles.FullJoin(destinationFiles,
            f => f,
            left => (FileDiff) new LeftOnlyDiff(left),
            right => new RightOnlyDiff(right),
            (left, right) => new BothDiff(left, right));
    }

    private static Task<Result<IEnumerable<FileWithMetadata>>> FilesWithMetadata(IZafiroDirectory origin)
    {
        var filesInTree = origin.GetFilesInTree();
        var result = filesInTree.BindMany(zafiroFile => { return zafiroFile.Hashes.Map(hashes => new { File = zafiroFile, Hashes = hashes }); }).BindMany(tuple => tuple.File.Properties.Map(properties => new FileWithMetadata { File = tuple.File, Properties = properties, Hashes = tuple.Hashes }));
        return result;
    }
}

public record FileWithMetadata
{
    public IZafiroFile File { get; set; }
    public FileProperties Properties { get; set; }
    public IDictionary<ChecksumKind, byte[]> Hashes { get; set; }
}

public record LeftOnlyDiff : FileDiff
{
    public FileWithMetadata Left { get; }

    public LeftOnlyDiff(FileWithMetadata left)
    {
        Left = left;
    }
}

public record RightOnlyDiff : FileDiff
{
    public FileWithMetadata Right { get; }

    public RightOnlyDiff(FileWithMetadata right)
    {
        Right = right;
    }
}

public record BothDiff : FileDiff
{
    public FileWithMetadata Left { get; }
    public FileWithMetadata Right { get; }

    public BothDiff(FileWithMetadata left, FileWithMetadata right)
    {
        Left = left;
        Right = right;
    }
}

public class FileSystemComparer
{
    public async Task<Result<IEnumerable<FileDiff>>> Diff(IZafiroDirectory origin, IZafiroDirectory destination)
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

    private static ZafiroPath GetKey(IZafiroDirectory origin, IZafiroFile f)
    {
        return f.Path.MakeRelativeTo(origin.Path);
    }
}