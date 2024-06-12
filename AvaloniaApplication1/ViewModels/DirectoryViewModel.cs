using Zafiro.FileSystem.DynamicData;

namespace AvaloniaApplication1.ViewModels;

public class DirectoryViewModel: IEntry
{
    public DynamicDirectory Parent { get; }
    public DynamicDirectory Directory { get; }

    public DirectoryViewModel(DynamicDirectory parent, DynamicDirectory directory)
    {
        Parent = parent;
        Directory = directory;
    }

    public string Name => Directory.Name;
    public string Key => Directory.Name + "/";
}