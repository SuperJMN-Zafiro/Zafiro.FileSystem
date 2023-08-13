using JetBrains.Annotations;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

[PublicAPI]
public class Fid
{
    public int volume_id { get; set; }
    public int file_key { get; set; }
    public long cookie { get; set; }
}