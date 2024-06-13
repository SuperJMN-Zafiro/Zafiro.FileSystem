using System.IO.Abstractions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using Zafiro.Avalonia.Mixins;
using Zafiro.Avalonia.Notifications;
using Zafiro.FileSystem.VNext;

namespace AvaloniaApplication1;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
        
        this.Connect(
            () => new MainView(), 
            mv =>
            {
                var notificationService = new NotificationService(new WindowNotificationManager(TopLevel.GetTopLevel(mv)));
                var fs = new LocalFileSystem(new FileSystem());
                var folder = fs.GetFolder("home/jmn/Escritorio");
                return new DirectoryContentsViewModel(notificationService, folder);
            }, () => new MainWindow());
    }
}