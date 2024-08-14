using CSharpFunctionalExtensions;
using DynamicData;
using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableDirectory : IMutableNode
{
    Task<Result> DeleteFile(string name);
    Task<Result> DeleteSubdirectory(string name);
    IObservable<IMutableFile> FileCreated { get; }
    IObservable<IMutableDirectory> DirectoryCreated { get; }
    IObservable<string> FileDeleted { get; }
    IObservable<string> DirectoryDeleted { get; }
    Task<Result<IMutableFile>> CreateFile(string entryName);
    Task<Result<IMutableDirectory>> CreateSubdirectory(string name);
    Task<Result<IEnumerable<IMutableNode>>> GetChildren(CancellationToken cancellationToken = default);
}