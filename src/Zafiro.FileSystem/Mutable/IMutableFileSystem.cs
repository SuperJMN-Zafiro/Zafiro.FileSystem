using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableFileSystem
{
    Task<Result<IRooted<IMutableDirectory>>> GetDirectory(ZafiroPath path);
    Task<Result<IRooted<IMutableFile>>> GetFile(ZafiroPath path);
    ZafiroPath InitialPath { get; }
}