using CSharpFunctionalExtensions;
using DynamicData;

namespace Zafiro.FileSystem.DynamicData;

public interface IDynamicDirectory : INamed
{
    IObservable<IChangeSet<IDynamicDirectory, string>> Directories { get; }
    IObservable<IChangeSet<IFile, string>> Files { get; }
    Task<Result> DeleteFile(string name);
    Task<Result> AddOrUpdateFile(params IFile[]  files);
}