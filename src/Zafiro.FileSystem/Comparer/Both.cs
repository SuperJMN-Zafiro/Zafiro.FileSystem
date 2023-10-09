namespace Zafiro.FileSystem.Comparer;

record Both(IZafiroFile Left, ZafiroPath Right) : FileDiff;