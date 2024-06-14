using Zafiro.Avalonia.FileExplorer.NextGen;
using Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;
using Zafiro.FileSystem.DynamicData;
using Zafiro.UI;

namespace AvaloniaApplication1.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(IFileSystem fileSystem, INotificationService notificationService)
    {
        FileExplorer = new FileExplorer(fileSystem, notificationService);
    }

    public FileExplorer FileExplorer { get; }
}