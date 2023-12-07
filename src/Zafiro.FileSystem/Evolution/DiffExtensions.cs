using Zafiro.FileSystem.Evolution.Comparer;

namespace Zafiro.FileSystem.Evolution;

public static class DiffExtensions
{
    public static IZafiroFile2 Get(this FileDiff diff, IZafiroDirectory2 origin)
    {
        return origin.FileSystem.GetFile(origin.Path.Combine(diff.Path));
    }

    public static (IZafiroFile2, IZafiroFile2) Get(this FileDiff diff, IZafiroDirectory2 origin, IZafiroDirectory2 destination)
    {
        return (diff.Get(origin), diff.Get(destination));
    }
}