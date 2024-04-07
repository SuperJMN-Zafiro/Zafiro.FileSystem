using Zafiro.FileSystem;

namespace ClassLibrary1;

public record DataEntry(ZafiroPath Path, IBlob Blob)
{
}