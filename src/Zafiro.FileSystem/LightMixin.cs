namespace Zafiro.FileSystem;

public static class LightMixin
{
    public static IEnumerable<IFile> Files(this IDirectory directory)
    {
        return directory.Children.OfType<IFile>();
    }

    public static IEnumerable<IRootedFile> RootedFiles(this IDirectory directory)
    {
        return directory.RootedFilesRelativeTo(ZafiroPath.Empty);
    }
    
    public static IEnumerable<IRootedFile> RootedFilesRelativeTo(this IDirectory directory, ZafiroPath path)
    {
        var myFiles = directory.Children.OfType<IFile>().Select(file => new RootedFile(path, file));
        var filesInSubDirs = directory.Directories().SelectMany(d => d.RootedFilesRelativeTo(path.Combine(d.Name)));
        
        return myFiles.Concat(filesInSubDirs);
    }
    
    public static IEnumerable<IDirectory> Directories(this IDirectory directory)
    {
        return directory.Children.OfType<IDirectory>();
    }
}