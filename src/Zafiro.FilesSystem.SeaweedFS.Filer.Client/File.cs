using JetBrains.Annotations;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

[PublicAPI]
public class File : BaseEntry
{
    public List<Chunk> Chunks { get; set; }
}