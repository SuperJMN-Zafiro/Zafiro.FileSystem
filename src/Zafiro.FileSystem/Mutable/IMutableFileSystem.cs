using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableFileSystem
{
    Task<Result<IRooted<IMutableDirectory>>> Get(ZafiroPath path);
    ZafiroPath InitialPath { get; }
}