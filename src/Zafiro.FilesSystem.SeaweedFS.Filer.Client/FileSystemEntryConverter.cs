using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class FileSystemEntryConverter : JsonConverter<BaseEntry>
{
    public override BaseEntry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            if (IsFile(root))
            {
                return JsonSerializer.Deserialize<File>(root.GetRawText(), options);
            }
            else
            {
                return JsonSerializer.Deserialize<Directory>(root.GetRawText(), options);
            }
        }
    }

    private bool IsFile(JsonElement root)
    {
        var hasSize = root.GetProperty("FileSize").GetUInt64() > 0;
        var hasMd5 = root.GetProperty("Md5").GetString() != null;
        var hasInode = root.GetProperty("Inode").GetUInt64() > 0;
        return hasSize || hasMd5 || hasInode;
    }

    public override void Write(Utf8JsonWriter writer, BaseEntry value, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Serialization is not supported in this custom converter.");
    }
}

public class BaseEntry
{
    public string FullPath { get; set; }
    public DateTime Mtime { get; set; }
    public DateTime Crtime { get; set; }
    public ulong Mode { get; set; }
    public ulong Uid { get; set; }
    public ulong Gid { get; set; }
    public string Mime { get; set; }
    public int TtlSec { get; set; }
    public string UserName { get; set; }
    public object GroupNames { get; set; }
    public string SymlinkTarget { get; set; }
    public object Md5 { get; set; }
    public long FileSize { get; set; }
    public int Rdev { get; set; }
    public ulong Inode { get; set; }
    public object Extended { get; set; }
    public object HardLinkId { get; set; }
    public ulong HardLinkCounter { get; set; }
    public object Content { get; set; }
    public object Remote { get; set; }
    public int Quota { get; set; }
}

public class File : BaseEntry
{
    public List<Chunk> Chunks { get; set; }
}

public class Directory : BaseEntry
{
    // Directory-specific properties can be added here, if available in the JSON
    public List<BaseEntry> Entries { get; set; }
}

public class Chunk
{
    public string file_id { get; set; }
    public int size { get; set; }
    public long modified_ts_ns { get; set; }
    public string e_tag { get; set; }
    public Fid fid { get; set; }
    public bool is_compressed { get; set; }
}

public class Fid
{
    public int volume_id { get; set; }
    public int file_key { get; set; }
    public long cookie { get; set; }
}

public class RootDirectory
{
    public string Path { get; set; }
    public List<BaseEntry> Entries { get; set; }
    public int Limit { get; set; }
    public string LastFileName { get; set; }
    public bool ShouldDisplayLoadMore { get; set; }
    public bool EmptyFolder { get; set; }
}
