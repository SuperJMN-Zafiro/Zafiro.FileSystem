namespace Zafiro.FileSystem;

public record FileProperties(bool IsHidden, DateTimeOffset CreationTime, long Length, IDictionary<ChecksumKind, byte[]> Checksum);


public enum ChecksumKind
{
    Invalid = 0,
    Md5
}