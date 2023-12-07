namespace Zafiro.FileSystem.Evolution.Comparer;

public record LeftOnly(ZafiroPath Left) : FileDiff(Left);