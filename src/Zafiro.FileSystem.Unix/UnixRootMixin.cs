namespace Zafiro.FileSystem.Unix;

public static class UnixRootMixin
{
    public static UnixNode FromRootedFiles(this ICollection<RootedUnixFile> files, ZafiroPath currentDirectory)
    {
        var rootFiles = files.Where(x => x.Parent == currentDirectory).Select(x => (UnixNode)x.UnixFile);
        var thisLevelDirs = files.Where(x => x.Parent.Parent() == currentDirectory).ToList();

        var nested = thisLevelDirs.Select(x => FromRootedFiles(files, x.Parent)).ToList();
        var unixNodes = rootFiles.Concat(nested);
        return new UnixDir(currentDirectory.Name(), unixNodes);
    }
}