using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace ClassLibrary1;

public static class Mixin
{
    public static Task<Result<IEnumerable<IFile>>> GetFilesInTree(this IDirectory directory)
    {
        return directory.GetDirectories()
            .Bind(async dirsResult =>
            {
                var tasks = dirsResult.Select(GetAll);
                var results = await Task.WhenAll(tasks);
                var combined = results.Combine();
                var merged = combined.Map(x => x.SelectMany(a => a));
                return merged;
            });
    }

    private static Task<Result<IEnumerable<IFile>>> GetAll(IDirectory d)
    {
        return d.GetFilesInTree().Bind(children => d.GetFiles().Map(children.Concat));
    }

    public static Task<Result<ZafiroPath>> GetPath(this IFile file)
    {
        var path = file.Parent.Bind(m => MaybeParent(file, m));

        return path;
    }

    private static Task<Result<ZafiroPath>> MaybeParent(IFile file, Maybe<IDirectory> m)
    {
        return m.Map(d => Map2(file, d)).GetValueOrDefault(() => Result.Success(ZafiroPath.Empty));
    }

    private static Task<Result<ZafiroPath>> Map2(IFile file, IDirectory d)
    {
        return d.GetPath()
            .Map(dirPath => ((ZafiroPath) file.Name).Combine(dirPath));
    }

    public static async Task<Result<ZafiroPath>> GetPath(this IDirectory directory)
    {
        var path = await directory
            .Parent.Bind(maybeParent => maybeParent.Map(async p =>
            {
                var result = await p.GetPath();
                return result.Map(zafiroPath => zafiroPath.Combine(p.Name));
            }).GetValueOrDefault(() => Result.Success(ZafiroPath.Empty)));

        return path;
    }
}