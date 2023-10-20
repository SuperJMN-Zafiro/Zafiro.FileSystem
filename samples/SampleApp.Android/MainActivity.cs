using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using Zafiro.FileSystem.Android;

namespace SampleApp.Android;

[Activity(
    Label = "SampleApp.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseReactiveUI();
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        AndroidPermissions.SetIsGranted(() => Environment.IsExternalStorageManager);
        AndroidPermissions.SetHandler(() => StartActivityForResult(new Intent(global::Android.Provider.Settings.ActionManageAllFilesAccessPermission), AndroidPermissions.RequestCode));
        base.OnCreate(savedInstanceState);
    }


    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        AndroidPermissions.OnResume();
        base.OnActivityResult(requestCode, resultCode, data);
    }
}
