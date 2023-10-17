using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Comparer;

namespace Zafiro.FileSystem.Synchronizer;

public class OverwriteRightFilesInBothSidesStrategy : IStrategy
{
    public Task<Result<IFileAction>> Create(IZafiroDirectory source, IZafiroDirectory destination)
    {
        return OverwriteRightFilesInBothSides.Create(source, destination).Cast(r => (IFileAction)r);
    }
}