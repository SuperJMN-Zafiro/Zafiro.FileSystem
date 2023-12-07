namespace Zafiro.FileSystem.Evolution.Comparer;

public record Both(ZafiroPath Path) : FileDiff(Path);