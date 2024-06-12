using System;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Reactive;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;
using Zafiro.Avalonia.Notifications;
using Zafiro.FileSystem;
using Zafiro.FileSystem.VNext;
using Zafiro.UI;

namespace AvaloniaApplication1.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(NotificationService notificationService)
    {
        var fs = new LocalFileSystem(new FileSystem());
        var folder = fs.GetFolder("home/jmn/Escritorio");

        var fileVms = folder.Files.Select(x => (IEntry)new FileViewModel(folder, x));
        var dirVms = folder.Directories.Select(x => (IEntry)new DirectoryViewModel(folder, x));

        var entries = fileVms.MergeChangeSets(dirVms);
        
        entries
            .Bind(out var itemCollection)
            .Subscribe();
        
        Items = itemCollection;
        CreateFile = ReactiveCommand.CreateFromTask(() => folder.AddOrUpdateFile(new File("Random", "Content")));
        CreateFile.HandleErrorsWith(notificationService);
    }

    public ReadOnlyObservableCollection<IEntry> Items { get; set; }

    public ReactiveCommand<Unit,Result> CreateFile { get; set; }

    public ReadOnlyObservableCollection<FileViewModel> Files { get; set; }
}