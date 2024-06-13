using System;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Disposables;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;
using Zafiro.Avalonia.Notifications;
using Zafiro.FileSystem;
using Zafiro.FileSystem.DynamicData;
using Zafiro.FileSystem.VNext;
using Zafiro.UI;

namespace AvaloniaApplication1.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly CompositeDisposable disposable = new();
    
    public MainViewModel(NotificationService notificationService, DynamicDirectory directory)
    {
        var fileVms = directory.Files.Select(x => (IEntry)new FileViewModel(directory, x));
        var dirVms = directory.Directories.Select(x => (IEntry)new DirectoryViewModel(directory, x));

        var entries = fileVms.MergeChangeSets(dirVms);
        
        entries
            .Bind(out var itemCollection)
            .Subscribe()
            .DisposeWith(disposable);
        
        Items = itemCollection;
        CreateFile = ReactiveCommand.CreateFromTask(() => directory.AddOrUpdateFile(new File("Random", "Content")));
        CreateFile
            .HandleErrorsWith(notificationService)
            .DisposeWith(disposable);
    }

    public ReadOnlyObservableCollection<IEntry> Items { get; set; }

    public ReactiveCommand<Unit,Result> CreateFile { get; set; }
}