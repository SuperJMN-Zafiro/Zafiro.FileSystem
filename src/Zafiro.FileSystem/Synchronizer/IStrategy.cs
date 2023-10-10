using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;

namespace Zafiro.FileSystem.Synchronizer;

public interface IStrategy
{
    Task<Result<IFileAction>> Create(IZafiroDirectory source, IZafiroDirectory destination);
}