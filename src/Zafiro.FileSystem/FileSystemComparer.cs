﻿using CSharpFunctionalExtensions;
using MoreLinq;

namespace Zafiro.FileSystem;

public interface IFileSystemComparer
{
    Task<Result<IEnumerable<Diff>>> Diff(IZafiroDirectory origin, IZafiroDirectory destination);
}

public class FileSystemComparer : IFileSystemComparer
{
    public async Task<Result<IEnumerable<Diff>>> Diff(IZafiroDirectory origin, IZafiroDirectory destination)
    {
        var originFiles = (await origin.GetFilesInTree()).Map(files => files.Select(file => GetKey(origin, file)));
        var destinationFiles = (await destination.GetFilesInTree()).Map(files => files.Select(file => GetKey(origin, file)));

        return from n in originFiles from q in destinationFiles select Join(n, q);
    }

    private IEnumerable<Diff> Join(IEnumerable<ZafiroPath> originFiles, IEnumerable<ZafiroPath> destinationFiles)
    {
        return originFiles.FullJoin(destinationFiles,
            f => f,
            left => new Diff(left, FileDiffStatus.LeftOnly),
            right => new Diff(right, FileDiffStatus.RightOnly),
            (left, right) => new Diff(left, FileDiffStatus.Both));
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

public class Diff
{
    public Diff(ZafiroPath path, FileDiffStatus status)
    {
        Path = path;
        Status = status;
    }

    public ZafiroPath Path { get; set; }
    public FileDiffStatus Status { get; set; }
}