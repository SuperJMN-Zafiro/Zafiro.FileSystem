namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class Directory : BaseEntry
{
    // Directory-specific properties can be added here, if available in the JSON
    public List<BaseEntry> Entries { get; set; } 
}