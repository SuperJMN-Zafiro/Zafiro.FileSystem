using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public static class ZafiroDirectoryMixin
{
    public static async Task<Result<IZafiroDirectory>> DescendantDirectory(this IZafiroDirectory directory, ZafiroPath subDirectoryPath)
    {
        // Base
        if (!subDirectoryPath.RouteFragments.Any())
        {
            return Result.Success(directory);
        }

        // Recursive
        var dirsResult = await directory.GetDirectories().ConfigureAwait(false);
        var tryFindResult = dirsResult.Map(r => r.TryFirst(x => DoMatch(x, subDirectoryPath)).ToResult($"Directory not found: {subDirectoryPath}"));
        var getDescendantResult = await tryFindResult.Bind(maybe => maybe.Bind(subDir => DescendantDirectory(subDir, NextPath(subDirectoryPath)))).ConfigureAwait(false);
        return getDescendantResult;
    }

    private static ZafiroPath NextPath(ZafiroPath subDirectoryPath)
    {
        return new ZafiroPath(subDirectoryPath.RouteFragments.Skip(1));
    }

    private static bool DoMatch(IZafiroDirectory zafiroDirectory, ZafiroPath subDirectoryPath)
    {
        return zafiroDirectory.Path.Name() == subDirectoryPath.RouteFragments.First();
    }

    public static Task<Result<IZafiroFile>> DescendantFile(this IZafiroDirectory dir, ZafiroPath file)
    {
        return dir
            .DescendantDirectory(file.Parent())
            .Bind(x => x.GetFile(file.Name()));
    }

    public static Task<Result<IZafiroFile>> GetFile(this IZafiroDirectory zafiroDirectory, string filename)
    {
        return zafiroDirectory
            .GetFiles()
            .Bind(r => r.TryFirst(file => string.Equals(file.Path.Name(), filename, StringComparison.OrdinalIgnoreCase)).ToResult($"File not found: {filename}"));
    }
}