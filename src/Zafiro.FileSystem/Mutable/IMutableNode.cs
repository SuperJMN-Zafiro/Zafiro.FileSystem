using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableNode : INode
{
    public bool IsHidden { get; }
    public Task<Result> Create();
}