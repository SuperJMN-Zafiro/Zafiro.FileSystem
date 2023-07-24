using MoreLinq;

namespace Zafiro.FileSystem;

public static class ZafiroPathExtensions
{
    public static ZafiroPath MakeRelativeTo(this ZafiroPath path, ZafiroPath relativeTo)
    {
        var relativePathChunks =
            relativeTo.RouteFragments
                .ZipLongest(path.RouteFragments, (x, y) => (x, y))
                .SkipWhile(x => x.x == x.y)
                .Select(x => { return x.x is null ? new[] {x.y} : new[] {"..", x.y}; })
                .Transpose()
                .SelectMany(x => x)
                .Where(x => x is not default(string));

        return new ZafiroPath(relativePathChunks);
    }
    
    public static ZafiroPath Combine(this ZafiroPath self, ZafiroPath path)
    {
        return new ZafiroPath(self.RouteFragments.Concat(path.RouteFragments));
    }
}