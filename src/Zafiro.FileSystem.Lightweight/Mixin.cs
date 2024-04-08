using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Lightweight;

public static class Mixin
{
    public static async Task<Result<IEnumerable<(ZafiroPath path, IBlob blob)>>> GetBlobsInTree(this IBlobContainer blobContainer, ZafiroPath currentPath)
    {
        var traverse = await blobContainer.Traverse(currentPath, (tree, path) =>
        {
            return tree.Blobs().Map(datas => datas.Select(r => (path.Combine(r.Name), r)));
        });

        Result<IEnumerable<(ZafiroPath, IBlob blob)>> paths = traverse.Map(enumerable => enumerable.SelectMany(x => x));

        return paths;
    }

    public static async Task<Result<IEnumerable<T>>> Traverse<T>(
        this IBlobContainer blobContainer, 
        ZafiroPath currentPath, 
        Func<IBlobContainer, ZafiroPath, Task<Result<T>>> onNode)
    {
        return await onNode(blobContainer, currentPath)
            .Bind(currentNode => blobContainer.Children()
                .Bind(children => TraverseChildren(blobContainer, children, currentPath, onNode, new List<T> { currentNode })));
    }

    private static async Task<Result<IEnumerable<T>>> TraverseChildren<T>(
        IBlobContainer blobContainer, 
        IEnumerable<IBlobContainer> children, 
        ZafiroPath currentPath, 
        Func<IBlobContainer, ZafiroPath, Task<Result<T>>> onNode,
        List<T> acc)
    {
        foreach (var child in children)
        {
            var childPath = currentPath.Combine(child.Name);
            var result = await Traverse(blobContainer, childPath, onNode);
            if (result.IsFailure)
            {
                return Result.Failure<IEnumerable<T>>(result.Error);
            }
            acc.AddRange(result.Value);
        }
        return Result.Success(acc.AsEnumerable());
    }

}