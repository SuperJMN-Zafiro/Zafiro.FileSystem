using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;

using SampleApp.ViewModels;
using SampleApp.Views;
using Zafiro.Avalonia.Mixins;
using Zafiro.Avalonia.Notifications;

namespace SampleApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        this.Connect(() => new MainView(), control => new MainViewModel(new NotificationService(new WindowNotificationManager(TopLevel.GetTopLevel(control)))));
        base.OnFrameworkInitializationCompleted();
    }
}
