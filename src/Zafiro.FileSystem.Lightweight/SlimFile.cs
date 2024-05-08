namespace Zafiro.FileSystem.Lightweight;

public class SlimFile : IFile
{
    public SlimFile(string name, IData data)
    {
        Name = name;
        Data = data;
    }

    public string Name { get; }
    public IData Data { get; }
    public IObservable<byte[]> Bytes => Data.Bytes;
    public long Length => Data.Length;
}

public static class SlimFileMixin
{
    public static IEnumerable<IFile> Files(this ISlimDirectory directory)
    {
        return directory.Children.OfType<IFile>();
    }
    
    public static IEnumerable<ISlimDirectory> Directories(this ISlimDirectory directory)
    {
        return directory.Children.OfType<ISlimDirectory>();
    }
}