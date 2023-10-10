namespace Zafiro.FileSystem.Comparer;

public record Both(ZafiroPath Path) : FileDiff(Path);