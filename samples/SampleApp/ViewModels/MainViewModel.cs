using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.FileSystem.Android;
using Zafiro.UI;

namespace SampleApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(INotificationService notificationService)
    {
        var fs = new AndroidFileSystem(new FileSystem(), Maybe<ILogger>.None);
        Explorer = new FileSystemExplorer(fs, notificationService, new ClipboardViewModel(), new TransferManagerViewModel());
    }

    public FileSystemExplorer Explorer { get; set; }
}