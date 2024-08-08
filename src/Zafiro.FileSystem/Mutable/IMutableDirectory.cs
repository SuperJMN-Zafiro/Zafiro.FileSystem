using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableDirectory : IMutableNode
{
    Task<Result> DeleteFile(string name);
    Task<Result> DeleteSubdirectory(string name);
    IObservable<Result<IEnumerable<IMutableNode>>> Children { get; }
    Task<Result<IMutableFile>> CreateFile(string entryName);
    Task<Result<IMutableDirectory>> CreateSubdirectory(string name);
}