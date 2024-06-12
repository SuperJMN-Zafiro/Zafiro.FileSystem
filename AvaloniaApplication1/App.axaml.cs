using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using Zafiro.Avalonia.Mixins;
using Zafiro.Avalonia.Notifications;

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
            mv => new MainViewModel(new NotificationService(new WindowNotificationManager(TopLevel.GetTopLevel(mv)))), () => new MainWindow());
    }
}