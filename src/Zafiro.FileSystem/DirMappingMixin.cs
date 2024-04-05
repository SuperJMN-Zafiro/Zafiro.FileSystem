namespace Zafiro.FileSystem;

public static class DirMappingMixin
{
    public static IZafiroDirectory RelativeTo(this IZafiroDirectory directory, IZafiroDirectory root)
    {
        Func<ZafiroPath, ZafiroPath> translatePath = path => path.MakeRelativeTo(root.Path);
        return new TranslateDirectory(directory, root, translatePath, d => d.RelativeTo(root), f => new TranslatedFile(f, root, translatePath));
    }

    public static IZafiroDirectory RelativeToItself(this IZafiroDirectory directory) => directory.RelativeTo(directory);

    public static IZafiroDirectory CombineWith(this IZafiroDirectory directory, IZafiroDirectory root)
    {
        Func<ZafiroPath, ZafiroPath> translatePath = path => root.Path.Combine(path);
        return new TranslateDirectory(directory, root, translatePath, d => d.CombineWith(root), f => new TranslatedFile(f, root, translatePath));
    }

    public static IZafiroDirectory MountIn(this IZafiroDirectory directory, IZafiroDirectory root)
    {
        var relative = directory.RelativeTo(directory);
        return relative.CombineWith(root);
    }
}