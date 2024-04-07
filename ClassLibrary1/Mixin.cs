using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Comparer;

namespace ClassLibrary1;

public static class Mixin
{
    public static Task<Result<IEnumerable<DataNode>>> GetAllNodes(this IDataTree dataTree, ZafiroPath path)
    {
        return dataTree.GetDirectories().MapMany(r => r with { Name = path.Combine(r.Name) });
    }

    public static async Task<Result<IEnumerable<DataEntry>>> GetAllEntries(this IDataTree dataTree, ZafiroPath path)
    {
        var getFilesResult = await dataTree.GetFiles().MapMany(x => x with { Path = path.Combine(x.Path) });
        var getAllFilesFromSubNodes = await dataTree
            .GetAllNodes(path)
            .Bind(namesAndNodes => namesAndNodes
                .Select(nameAndNode => nameAndNode.Node.GetAllEntries(nameAndNode.Name))
                .Combine()
                .Map(listsOfDataEntries => listsOfDataEntries.SelectMany(dataEntries => dataEntries)));

        return getFilesResult.CombineAndMap(getAllFilesFromSubNodes, (a, b) => a.Concat(b));
    }
}