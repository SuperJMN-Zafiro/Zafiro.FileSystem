namespace Zafiro.FileSystem.Sftp;

public static class PathMixin
{
    public static string FromZafiroPath(this ZafiroPath path)
    {
        return "/" + path.Path;
    }

    public static string ToZafiroPath(this string path)
    {
        return path.StartsWith("/") ? path[1..] : path;
    }
}