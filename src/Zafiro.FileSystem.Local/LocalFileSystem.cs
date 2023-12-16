using Zafiro.FileSystem.Local.Android;

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

        if (OperatingSystem.IsAndroid())
        {
#if ANDROID
            return new AndroidFileSystem(new System.IO.Abstractions.FileSystem());
#endif
        }
        throw new NotSupportedException("The file system is not supported");
    }
}