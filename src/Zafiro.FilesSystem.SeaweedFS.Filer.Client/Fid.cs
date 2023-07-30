namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class Fid
{
    public int volume_id { get; set; }
    public int file_key { get; set; }
    public long cookie { get; set; }
}