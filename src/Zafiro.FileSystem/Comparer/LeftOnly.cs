namespace Zafiro.FileSystem.Comparer;

public record LeftOnly(ZafiroPath Left) : FileDiff(Left);