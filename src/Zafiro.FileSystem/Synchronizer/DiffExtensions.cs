using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Comparer;

namespace Zafiro.FileSystem.Synchronizer;

public static class DiffExtensions
{
    public static Task<Result<IZafiroFile>> Get(this FileDiff diff, IZafiroDirectory origin)
    {
        return origin.FileSystem.GetFile(origin.Path.Combine(diff.Path));
    }

    public static Task<Result<(IZafiroFile, IZafiroFile)>> Get(this FileDiff diff, IZafiroDirectory origin, IZafiroDirectory destination)
    {
        return diff.Get(origin).CombineAndMap(diff.Get(destination), (src, dst) => (src, dst));
    }
}