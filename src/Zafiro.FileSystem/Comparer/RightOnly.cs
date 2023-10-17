namespace Zafiro.FileSystem.Comparer;

public record RightOnly(ZafiroPath Right) : FileDiff(Right);