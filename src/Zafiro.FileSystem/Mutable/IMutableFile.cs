using CSharpFunctionalExtensions;
using Zafiro.DataModel;

namespace Zafiro.FileSystem.Mutable.Mutable;

public interface IMutableFile : IMutableNode
{
    Task<Result> SetContents(IData data, CancellationToken cancellationToken = default);
    Task<Result<IData>> GetContents();
    Task<Result> Delete();
}