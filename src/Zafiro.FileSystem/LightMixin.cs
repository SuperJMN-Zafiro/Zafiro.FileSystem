namespace Zafiro.FileSystem.Lightweight;

public static class LightMixin
{
    public static IEnumerable<IFile> Files(this IDirectory directory)
    {
        return directory.Children.OfType<IFile>();
    }
    
    public static IEnumerable<IRootedFile> FilesInTree(this IDirectory directory, ZafiroPath path)
    {
        var myFiles = directory.Children.OfType<IFile>().Select(file => new RootedFile(path, file));
        var filesInSubDirs = directory.Directories().SelectMany(d => d.FilesInTree(path.Combine(d.Name)));
        
        return myFiles.Concat(filesInSubDirs);
    }
    
    public static IEnumerable<IDirectory> Directories(this IDirectory directory)
    {
        return directory.Children.OfType<IDirectory>();
    }
}