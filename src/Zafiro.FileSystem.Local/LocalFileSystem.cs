namespace Zafiro.FileSystem.Local;

public static class LocalFileSystem
{
    public static IZafiroFileSystem Create()
    {
        if (OperatingSystem.IsWindows())
        {
            return new WindowsZafiroFileSystem(new System.IO.Abstractions.FileSystem());
        }

        if (OperatingSystem.IsLinux())
        {
            return new LinuxZafiroFileSystem(new System.IO.Abstractions.FileSystem());
        }

        throw new NotSupportedException("The file system is not supported");
    }
}