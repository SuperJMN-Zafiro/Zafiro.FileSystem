namespace Zafiro.FileSystem;

public class DirectoryProperties
{
    public DirectoryProperties(bool isHidden, DateTime creationTime)
    {
        IsHidden = isHidden;
        CreationTime = creationTime;
    }

    public DateTime CreationTime { get; }
    public bool IsHidden { get; }
}