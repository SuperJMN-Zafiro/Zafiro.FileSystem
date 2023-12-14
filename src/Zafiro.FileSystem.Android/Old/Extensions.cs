namespace Zafiro.FileSystem.Android.Old;

public static class Extensions
{
    public static ZafiroPath FromAndroidToZafiro(this string androidPath)
    {
        string unRooted;
        if (androidPath.StartsWith("\\"))
        {
            unRooted = androidPath[1..];
        }
        else
        {
            unRooted = androidPath;
        }

        return unRooted.Replace("\\", "/");
    }

    public static string FromZafiroPath(this ZafiroPath zafiroPath)
    {
        return "\\" + zafiroPath.Path.Replace("/", "\\");
    }
}