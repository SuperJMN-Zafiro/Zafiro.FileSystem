using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace ClassLibrary1;

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
        // Aplicar la función al nodo actual
        var currentNodeResult = await onNode(blobContainer, currentPath);

        if (!currentNodeResult.IsSuccess)
        {
            // Si falla, detenemos el recorrido y devolvemos el error.
            return Result.Fail<IEnumerable<T>>(currentNodeResult.Error);
        }

        var results = new List<T> { currentNodeResult.Value };

        // Obtener hijos de forma asíncrona y procesar recursivamente
        var childrenResult = await blobContainer.Children();

        if (!childrenResult.IsSuccess)
        {
            // Manejar error al obtener hijos
            return Result.Fail<IEnumerable<T>>(childrenResult.Error);
        }

        foreach (var child in childrenResult.Value)
        {
            var childPath = currentPath.Combine(child.Name); // Asumiendo que ZafiroPath tiene un método Add o similar
            var childResult = await Traverse(child, childPath, onNode);

            if (childResult.IsSuccess)
            {
                results.AddRange(childResult.Value);
            }
            else
            {
                // Opcional: Decidir si un error en un hijo debe detener todo el proceso o no
                return Result.Fail<IEnumerable<T>>(childResult.Error);
            }
        }

        return Result.Ok(results.AsEnumerable());
    }
}