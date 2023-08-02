namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class RootDirectory
{
    public string Path { get; set; }
    public List<BaseEntry>? Entries { get; set; }
    public int Limit { get; set; }
    public string LastFileName { get; set; }
    public bool ShouldDisplayLoadMore { get; set; }
    public bool EmptyFolder { get; set; }
}