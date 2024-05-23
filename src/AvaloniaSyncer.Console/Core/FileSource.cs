using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Lightweight;

namespace AvaloniaSyncer.Console.Core;

public record FileSource
{
    public FileSource(ISyncFileSystem plugin, ZafiroPath path)
    {
        Plugin = plugin;
        Path = path;
    }

    public ISyncFileSystem Plugin { get; }
    public ZafiroPath Path { get; }

    public Task<Result<IDirectory>> GetFiles() => Plugin.GetFiles(Path);

    public override string ToString() => Plugin + "[" + Path + "]";
}