namespace Zafiro.FileSystem.Unix;

public static class UnixRootMixin
{
    public static UnixNode FromRootedFiles(this ICollection<RootedUnixFile> allFiles, ZafiroPath parent)
    {
        var files = allFiles.Where(x => x.Parent == parent).Select(x => (UnixNode)x.UnixFile);
        var nextLevelPaths = allFiles.Where(x => x.Parent.Parent() == parent).Select(x => x.Parent).ToList().Distinct();
        var dirs = nextLevelPaths.Select(x => FromRootedFiles(allFiles, x)).ToList();
        var unixNodes = files.Concat(dirs);
        return new UnixDir(parent.Name(), unixNodes);
    }
}