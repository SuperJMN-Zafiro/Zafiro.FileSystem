using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public static class ZafiroDirectoryMixin
{
    public static async Task<Result<Maybe<IZafiroDirectory>>> DescendantDirectory(this IZafiroDirectory directory, ZafiroPath subDirectoryPath)
    {
        // Base
        if (!subDirectoryPath.RouteFragments.Any())
        {
            return Result.Success(Maybe.From(directory));
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

    public static Task<Result<Maybe<IZafiroFile>>> GetFile(this IZafiroDirectory zafiroDirectory, string filename)
    {
        var maybeGetFile = zafiroDirectory
            .GetFiles()
            .Map(r => r.TryFirst(file => string.Equals(file.Path.Name(), filename, StringComparison.OrdinalIgnoreCase)));

        return maybeGetFile;
    }

    public static Task<Result<Maybe<IZafiroFile>>> DescendantFile(this IZafiroDirectory dir, ZafiroPath file)
    {
        return dir
            .DescendantDirectory(file.Parent())
            .Bind(maybe => maybe.Match(directory => directory.GetFile(file.Name()), () => Task.FromResult(Result.Success(Maybe<IZafiroFile>.None))));
    }

    public static Task<Result<IZafiroFile>> GetFromPath(this IZafiroDirectory origin, ZafiroPath path)
    {
        return origin.FileSystem.GetFile(origin.Path.Combine(path));
    }
}