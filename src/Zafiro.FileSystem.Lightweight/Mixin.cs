using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public static class Mixin
{
    public static async Task<Result<IEnumerable<(ZafiroPath Path, IFile Blob)>>> GetFilesInTree(this IDirectory directory, ZafiroPath currentPath)
    {
        var traverse = await directory.Traverse(currentPath, (tree, path) =>
        {
            return tree.Files().Map(datas => datas.Select(r => (path, r)));
        });

        Result<IEnumerable<(ZafiroPath, IFile blob)>> paths = traverse.Map(enumerable => enumerable.SelectMany(x => x));

        return paths;
    }

    public static async Task<Result<IEnumerable<T>>> Traverse<T>(
        this IDirectory directory, 
        ZafiroPath currentPath, 
        Func<IDirectory, ZafiroPath, Task<Result<T>>> onNode)
    {
        return await onNode(directory, currentPath)
            .Bind(currentNode => directory.Directories()
                .Bind(children => TraverseChildren(children, currentPath, onNode, new List<T> { currentNode })));
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

}