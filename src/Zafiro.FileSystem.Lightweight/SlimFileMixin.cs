namespace Zafiro.FileSystem.Lightweight;

public static class SlimFileMixin
{
    public static IEnumerable<IFile> Files(this ISlimDirectory directory)
    {
        return directory.Children.OfType<IFile>();
    }
    
    public static IEnumerable<ISlimDirectory> Directories(this ISlimDirectory directory)
    {
        return directory.Children.OfType<ISlimDirectory>();
    }
}