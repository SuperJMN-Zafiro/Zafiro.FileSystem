using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Lightweight;

namespace AvaloniaSyncer.Console;

public interface IFileSystemPlugin
{
    string Name { get; }
    string DisplayName { get; }
    Task<Result<IDirectory>> GetFiles(ZafiroPath path);
    Task<Result> Copy(IFile left, ZafiroPath destination);
}