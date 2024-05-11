namespace Zafiro.FileSystem.Lightweight;

public static class Mixin
{
    public static IEnumerable<IFile> Files(this IDirectory directory)
    {
        return directory.Children.OfType<IFile>();
    }
    
    public static IEnumerable<IFile> FilesInTree(this IDirectory directory)
    {
        var myFiles = directory.Children.OfType<IFile>();
        var filesInSubDirs = directory.Directories().SelectMany(d => d.FilesInTree());
        
        return myFiles.Concat(filesInSubDirs);
    }
    
    public static IEnumerable<IDirectory> Directories(this IDirectory directory)
    {
        return directory.Children.OfType<IDirectory>();
    }
}