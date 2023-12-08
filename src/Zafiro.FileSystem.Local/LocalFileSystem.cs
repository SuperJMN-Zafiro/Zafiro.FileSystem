namespace Zafiro.FileSystem.Local;

public class LocalFileSystem
{
    public IZafiroFileSystem Create()
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