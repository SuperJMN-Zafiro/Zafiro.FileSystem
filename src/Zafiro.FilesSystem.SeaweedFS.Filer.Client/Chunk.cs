namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class Chunk
{
    public string file_id { get; set; }
    public int size { get; set; }
    public long modified_ts_ns { get; set; }
    public string e_tag { get; set; }
    public Fid fid { get; set; }
    public bool is_compressed { get; set; }
}