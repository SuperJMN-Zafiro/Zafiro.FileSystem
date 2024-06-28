using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableFileSystem
{
    Task<Result<IMutableDirectory>> GetDirectory(ZafiroPath path);
    Task<Result<IMutableFile>> GetFile(ZafiroPath path);
    ZafiroPath InitialPath { get; }
}