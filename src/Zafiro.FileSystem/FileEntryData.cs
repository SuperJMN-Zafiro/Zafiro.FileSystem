namespace Zafiro.FileSystem;

internal class FileEntryData
{
    public IZafiroFile ZafiroFile { get; }
    public long Size { get; }

    public FileEntryData(IZafiroFile zafiroFile, long size)
    {
        ZafiroFile = zafiroFile;
        Size = size;
    }
}