using CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Lightweight;

namespace AvaloniaSyncer.Console;

public record FileSource
{
    public FileSource(IFileSystemPlugin plugin, ZafiroPath path)
    {
        Plugin = plugin;
        Path = path;
    }

    public IFileSystemPlugin Plugin { get; }
    public ZafiroPath Path { get; }

    public Task<Result<IDirectory>> GetFiles() => Plugin.GetFiles(Path);

    public override string ToString() => Plugin + ":" + Path;
}