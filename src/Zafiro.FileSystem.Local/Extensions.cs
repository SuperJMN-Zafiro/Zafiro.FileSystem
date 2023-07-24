namespace Zafiro.FileSystem.Local;

public static class Extensions
{
    public static ZafiroPath ToZafiroPath(this string windowsPath)
    {
        return windowsPath.Replace("\\", "/");
    }
}