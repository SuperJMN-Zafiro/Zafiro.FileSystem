using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Lightweight;

namespace AvaloniaSyncer.Console;

public interface IFileSystemPlugin
{
    string Name { get; set; }
    string DisplayName { get; set; }
    Task<Result<IDirectory>> GetFiles(ZafiroPath path);
}