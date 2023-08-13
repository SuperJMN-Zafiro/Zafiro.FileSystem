using JetBrains.Annotations;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

[PublicAPI]
public class RootDirectory
{
    public string Path { get; set; }
    public List<BaseEntry>? Entries { get; set; }
    public int Limit { get; set; }
    public string LastFileName { get; set; }
    public bool ShouldDisplayLoadMore { get; set; }
    public bool EmptyFolder { get; set; }
}