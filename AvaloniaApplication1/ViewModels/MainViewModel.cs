using System;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Reactive;
using System.Windows.Input;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;
using Zafiro.Avalonia.Notifications;
using Zafiro.FileSystem;
using Zafiro.FileSystem.DynamicData;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.VNext;
using Zafiro.UI;
using IFile = Zafiro.FileSystem.IFile;

namespace AvaloniaApplication1.ViewModels;

public class MainViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    public string Greeting => "Welcome to Avalonia!";
#pragma warning restore CA1822 // Mark members as static

    public MainViewModel(NotificationService notificationService)
    {
        var fs = new LocalFileSystem(new FileSystem());
        var folder = fs.GetFolder("home/jmn/Escritorio");
        
        folder.Files
            .Select(x => new FileViewModel(folder, x))
            .Bind(out var files)
            .Subscribe();
        
        Files = files;
        CreateFile = ReactiveCommand.CreateFromTask(() => folder.AddOrUpdate(new File("Random", "Content")));
        CreateFile.HandleErrorsWith(notificationService);
    }

    public ReactiveCommand<Unit,Result> CreateFile { get; set; }

    public ReadOnlyObservableCollection<FileViewModel> Files { get; set; }
}

public class FileViewModel
{
    public IFile File { get; }

    public FileViewModel(DynamicDirectory directory, IFile file)
    {
        File = file;
        Delete = ReactiveCommand.CreateFromTask(() => directory.Delete(Name));
    }

    public string Name => File.Name;
    public ICommand Delete { get; }
}

