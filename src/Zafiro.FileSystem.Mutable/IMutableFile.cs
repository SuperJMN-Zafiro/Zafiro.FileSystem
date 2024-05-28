using CSharpFunctionalExtensions;
using Zafiro.DataModel;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableFile : IMutableNode
{
    Task<Result> SetContents(IData data);
}