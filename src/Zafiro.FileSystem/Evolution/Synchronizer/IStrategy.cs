using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;

namespace Zafiro.FileSystem.Evolution.Synchronizer;

public interface IStrategy
{
    Task<Result<IFileAction>> Create(IZafiroDirectory2 source, IZafiroDirectory2 destination);
}