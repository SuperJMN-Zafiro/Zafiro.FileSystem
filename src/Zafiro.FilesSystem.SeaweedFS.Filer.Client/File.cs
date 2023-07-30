namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class File : BaseEntry
{
    public List<Chunk> Chunks { get; set; }
}