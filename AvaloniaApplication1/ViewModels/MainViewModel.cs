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
        
        folder.Directories
            .Select(x => new DirectoryViewModel(folder, x))
            .Bind(out var directories)
            .Subscribe();
        
        Files = files;
        Directories = directories;
        CreateFile = ReactiveCommand.CreateFromTask(() => folder.AddOrUpdateFile(new File("Random", "Content")));
        CreateFile.HandleErrorsWith(notificationService);
    }

    public ReadOnlyObservableCollection<DirectoryViewModel> Directories { get; set; }

    public ReactiveCommand<Unit,Result> CreateFile { get; set; }

    public ReadOnlyObservableCollection<FileViewModel> Files { get; set; }
}

public class DirectoryViewModel
{
    public DynamicDirectory Parent { get; }
    public DynamicDirectory Directory { get; }

    public DirectoryViewModel(DynamicDirectory parent, DynamicDirectory directory)
    {
        Parent = parent;
        Directory = directory;
    }

    public string Name => Directory.Name;
}

public class FileViewModel
{
    public IFile File { get; }

    public FileViewModel(DynamicDirectory directory, IFile file)
    {
        File = file;
        Delete = ReactiveCommand.CreateFromTask(() => directory.DeleteFile(Name));
    }

    public string Name => File.Name;
    public ICommand Delete { get; }
}

