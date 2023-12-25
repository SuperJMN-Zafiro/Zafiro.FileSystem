namespace Zafiro.FileSystem;

public record FileProperties(bool IsHidden, DateTimeOffset CreationTime, long Length);