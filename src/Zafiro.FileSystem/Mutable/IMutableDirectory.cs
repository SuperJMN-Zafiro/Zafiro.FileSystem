using System.Reactive.Subjects;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Mutable.Mutable;

public interface IMutableDirectory : IMutableNode, IAsyncDir
{
    Task<Result<IEnumerable<IMutableNode>>> MutableChildren();
    Task<Result> AddOrUpdate(IFile data, ISubject<double>? progress = null);
    Task<Result<IMutableFile>> CreateFile(string name);
    Task<Result<IMutableDirectory>> CreateDirectory(string name);
}