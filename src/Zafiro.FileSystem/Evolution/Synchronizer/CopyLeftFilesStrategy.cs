using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;

namespace Zafiro.FileSystem.Evolution.Synchronizer;

public class CopyLeftFilesStrategy : IStrategy
{
    public Task<Result<IFileAction>> Create(IZafiroDirectory2 source, IZafiroDirectory2 destination)
    {
        return CopyLeftFilesToRightSideAction.Create(source, destination).Cast(r => (IFileAction)r);
    }
}