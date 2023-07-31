namespace Zafiro.FileSystem.Local;

public static class Extensions
{
    public static ZafiroPath ToZafiroPath(this string windowsPath)
    {
        return windowsPath.Replace("\\", "/");
    }

    public static string FromZafiroPath(this ZafiroPath zafiroPath)
    {
        return zafiroPath.Path.Replace("/", "\\");
    }
}