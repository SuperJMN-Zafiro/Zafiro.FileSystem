namespace Zafiro.FileSystem.Evolution.Comparer;

public record RightOnly(ZafiroPath Right) : FileDiff(Right);