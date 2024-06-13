using CSharpFunctionalExtensions;
using Zafiro.Reactive;

namespace Zafiro.FileSystem;

public static class Mixin
{
    public static Task<Result<IEnumerable<IFile>>> Files(this IHeavyDirectory heavyDirectory)
    {
        return heavyDirectory.Children().Map(nodes => nodes.OfType<IFile>());
    }
    
    public static Task<Result<IEnumerable<IHeavyDirectory>>> Directories(this IHeavyDirectory heavyDirectory)
    {
        return heavyDirectory.Children().Map(nodes => nodes.OfType<IHeavyDirectory>());
    }
    
    public static async Task<Result<IEnumerable<IRootedFile>>> GetFilesInTree(this IHeavyDirectory heavyDirectory, ZafiroPath currentPath)
    {
        var traverse = await heavyDirectory.Traverse(currentPath, (tree, path) =>
        {
            return tree.Files().Map(datas => datas.Select(r => (IRootedFile) new RootedFile(path, r)));
        });

        var paths = traverse.Map(enumerable => enumerable.SelectMany(x => x));

        return paths;
    }

    public static Task<Result<IEnumerable<T>>> Traverse<T>(
        this IHeavyDirectory heavyDirectory, 
        ZafiroPath currentPath, 
        Func<IHeavyDirectory, ZafiroPath, Task<Result<T>>> onNode)
    {
        return onNode(heavyDirectory, currentPath)
            .Bind(currentNode => heavyDirectory.Directories()
                .Bind(children => TraverseChildren(children, currentPath, onNode, [currentNode])));
    }

    private static async Task<Result<IEnumerable<T>>> TraverseChildren<T>(IEnumerable<IHeavyDirectory> children, 
        ZafiroPath currentPath, 
        Func<IHeavyDirectory, ZafiroPath, Task<Result<T>>> onNode,
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

    public static ZafiroPath FullPath<T>(this IRooted<T> rootedFile) where T : INamed
    {
        return rootedFile.Path.Combine(rootedFile.Value.Name);
    }
    
    public static Task<Result<IDirectory>> ToDirectory(this IAsyncDir asyncDir)
    {
        return asyncDir.Children()
            .Map(nodes => nodes.ToList())
            .Bind(async nodes =>
            {
                var dirs = nodes.OfType<IAsyncDir>();
                var files = nodes.OfType<IFile>().Cast<INode>();
                var dirResult = await dirs.Select(ToDirectory).Combine();
                return dirResult.Map(d => d.Concat(files));
            })
            .Map(enumerable => (IDirectory)new Directory(asyncDir.Name, enumerable));
    }

    public static Stream ToStream(this IFile file)
    {
        return file.Bytes.ToStream();
    }
}