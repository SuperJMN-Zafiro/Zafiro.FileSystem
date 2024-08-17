using System.Reactive.Concurrency;
using CSharpFunctionalExtensions;
using Zafiro.DataModel;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableFile : IMutableNode
{
    Task<Result> SetContents(IData data, CancellationToken cancellationToken = default, IScheduler? scheduler = null);
    Task<Result<IData>> GetContents();
}