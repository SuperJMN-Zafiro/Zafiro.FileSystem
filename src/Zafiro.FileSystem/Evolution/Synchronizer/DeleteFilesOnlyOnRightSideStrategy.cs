using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;

namespace Zafiro.FileSystem.Evolution.Synchronizer;

public class DeleteFilesOnlyOnRightSideStrategy : IStrategy
{
    public Task<Result<IFileAction>> Create(IZafiroDirectory2 source, IZafiroDirectory2 destination)
    {
        return DeleteFilesOnlyOnRightSide.Create(source, destination).Cast(r => (IFileAction)r);
    }
}