namespace Zafiro.FileSystem.SeaweedFS;

public static class SeaweedPathMixin
{
    public static string ToServicePath(this ZafiroPath path)
    {
        return "/" + path;
    }

    public static ZafiroPath ToZafiroPath(this string path)
    {
        if (path.StartsWith("/"))
        {
            return path[1..];
        }

        return path;
    }
}