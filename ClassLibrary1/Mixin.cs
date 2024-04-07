using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Comparer;

namespace ClassLibrary1;

public static class Mixin
{
    public static Task<Result<IEnumerable<IData>>> GetFilesInTree(this IDataTree dataTree)
    {
        return dataTree.GetFilesAndPaths().MapMany(f => f.Item1);
    }

    public static Task<Result<IEnumerable<(IData, ZafiroPath)>>> GetFilesAndPaths(this IDataTree dataTree)
    {
        return dataTree.GetDirectories()
            .Bind(async dirsResult =>
            {
                var tasks = dirsResult.Select(GetAll);
                var results = await Task.WhenAll(tasks);
                var combined = results.Combine();
                var merged = combined.Map(x => x.SelectMany(a => a.Select(file => (file, ((ZafiroPath)dataTree.Name).Combine(file.Name)))));
                return merged;
            });
    }

    private static Task<Result<IEnumerable<IData>>> GetAll(IDataTree d)
    {
        return d.GetFilesInTree().Bind(children => d.GetFiles().Map(children.Concat));
    }
}