using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public static class Mixin
{
    public static async Task<Result<IEnumerable<RootedFile>>> GetFilesInTree(this IDirectory directory, ZafiroPath currentPath)
    {
        var traverse = await directory.Traverse(currentPath, (tree, path) =>
        {
            return tree.Files().Map(datas => datas.Select(r => new RootedFile(path, r)));
        });

        var paths = traverse.Map(enumerable => enumerable.SelectMany(x => x));

        return paths;
    }

    public static Task<Result<IEnumerable<T>>> Traverse<T>(
        this IDirectory directory, 
        ZafiroPath currentPath, 
        Func<IDirectory, ZafiroPath, Task<Result<T>>> onNode)
    {
        return onNode(directory, currentPath)
            .Bind(currentNode => directory.Directories()
                .Bind(children => TraverseChildren(children, currentPath, onNode, [currentNode])));
    }

    private static async Task<Result<IEnumerable<T>>> TraverseChildren<T>(IEnumerable<IDirectory> children, 
        ZafiroPath currentPath, 
        Func<IDirectory, ZafiroPath, Task<Result<T>>> onNode,
        List<T> acc)
    {
        foreach (var child in children)
        {
            var childPath = currentPath.Combine(child.Name);
            var result = await Traverse(child, childPath, onNode);
            if (result.IsFailure)
            {
                return Result.Failure<IEnumerable<T>>(result.Error);
            }
            acc.AddRange(result.Value);
        }
        return Result.Success(acc.AsEnumerable());
    }

    public static ZafiroPath FullPath(this RootedFile rootedFile)
    {
        return rootedFile.Path.Combine(rootedFile.File.Name);
    }
}