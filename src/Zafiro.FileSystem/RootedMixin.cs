namespace Zafiro.FileSystem;

public static class RootedMixin
{
    public static IZafiroDirectory AsRooted(this IZafiroDirectory directory)
    {
        return new RootedDirectory(directory, directory);
    }
}