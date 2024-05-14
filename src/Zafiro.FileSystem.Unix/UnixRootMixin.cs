using Zafiro.Mixins;

namespace Zafiro.FileSystem.Unix;

public static class UnixRootMixin
{
    // TODO: Esto no funciona
    public static UnixNode FromRootedFiles(this ICollection<RootedUnixFile> allFiles, ZafiroPath parent)
    {
        var files = allFiles.Where(x => x.Path == parent).Select(x => (UnixNode)x.UnixFile);
        var allParents = allFiles.Select(x => x.Path.ParentsAndSelf()).Flatten();

        var nextLevelPaths = allParents
            .Where(x => x.RouteFragments.Count() == parent.RouteFragments.Count() + 1)
            .Select(x => x.Path).ToList().Distinct();

        var dirs = nextLevelPaths.Select(x => FromRootedFiles(allFiles, x)).ToList();
        var unixNodes = files.Concat(dirs);
        return new UnixDir(parent.Name(), unixNodes);
    }
}