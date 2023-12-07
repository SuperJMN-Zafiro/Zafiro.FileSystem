using Zafiro.FileSystem.Comparer;

namespace Zafiro.FileSystem;

public static class DiffExtensions
{
    public static IZafiroFile Get(this FileDiff diff, IZafiroDirectory origin)
    {
        return origin.FileSystem.GetFile(origin.Path.Combine(diff.Path));
    }

    public static (IZafiroFile, IZafiroFile) Get(this FileDiff diff, IZafiroDirectory origin, IZafiroDirectory destination)
    {
        return (diff.Get(origin), diff.Get(destination));
    }
}