namespace Zafiro.FileSystem.Comparer;

record Both(IZafiroFile Left, IZafiroFile Right) : FileDiff;