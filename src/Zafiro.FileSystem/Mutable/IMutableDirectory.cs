using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableDirectory : IMutableNode, IAsyncDir
{
    Task<Result<IEnumerable<IMutableNode>>> MutableChildren();
    Task<Result<IMutableDirectory>> CreateSubdirectory(string name);
    Task<Result> Delete();
    IObservable<Result<IEnumerable<IMutableNode>>> ChildrenProp { get; }
}