using Serilog;
#if ANDROID
using Android.Content;
using AppResult = Android.App.Result;
#endif

namespace Zafiro.FileSystem.Android.Old;

public class AndroidFileSystem : IFileSystem
{
    private readonly System.IO.Abstractions.IFileSystem fileSystem;
    private readonly Maybe<ILogger> logger;
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    private static bool isInitialized;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

    public AndroidFileSystem(System.IO.Abstractions.IFileSystem fileSystem, Maybe<ILogger> logger)
    {
        EnsureInitialized();
        this.fileSystem = fileSystem;
        this.logger = logger;
    }

    public Task<Result<IZafiroDirectory>> GetDirectory(ZafiroPath path)
    {
        return AndroidPermissions.Request().Bind(() =>
        {
            if (path == ZafiroPath.Empty)
            {
                return Task.FromResult(Result.Success<IZafiroDirectory>(new RootDirectory(this, fileSystem, logger)));
            }

            return Task.FromResult(Result.Try<IZafiroDirectory>(() =>
            {
                var localPath = "/" + path;
                var directoryInfo = fileSystem.DirectoryInfo.New(localPath);
                return new AndroidDirectory(directoryInfo, logger, this);
            }, ex => ExceptionHandler.HandleError(path, ex, logger)));
        });
    }

    public Task<Result<IZafiroFile>> GetFile(ZafiroPath path)
    {
        return AndroidPermissions.Request()
            .Bind(() => Result.Try<IZafiroFile>(() => new AndroidFile(fileSystem.FileInfo.New(path), logger), ex => ExceptionHandler.HandleError(path, ex, logger)));
    }

    public Task<Result<ZafiroPath>> GetRoot()
    {
#if ANDROID
        return AndroidPermissions.Request().Bind(() =>
        {
            var path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            return ZafiroPath.Create(path[1..]);
        });
#endif
        return Task.FromResult(Result.Failure<ZafiroPath>("Not supported"));
    }

    private void EnsureInitialized()
    {
        if (!isInitialized)
        {
            throw new InvalidOperationException("AndroidFilesSystem has not been initialized. Please, call AndroidFileSystem.Register(this) in you main Activity and override OnActivityResult to call AndroidFileSystem.OnActivityResult(...) in it.");
        }
    }

#if ANDROID
    public static void OnActivityResult(int requestCode, AppResult resultCode, Intent? data)
    {
        AndroidPermissions.OnActivityResult(requestCode, resultCode, data);
    }

    public static void Register(Activity activity)
    {
        AndroidPermissions.SetIsGranted(() => global::Android.OS.Environment.IsExternalStorageManager);
        AndroidPermissions.SetHandler(() => activity.StartActivityForResult(new Intent(global::Android.Provider.Settings.ActionManageAllFilesAccessPermission), AndroidPermissions.RequestCode));
        isInitialized = true;
    }
#endif

#if ANDROID
    public static void OnActivityResult(int requestCode, AppResult resultCode, Intent? data)
    {
        AndroidPermissions.OnActivityResult(requestCode, resultCode, data);
    }

    public static void Register(Activity activity)
    {
        AndroidPermissions.SetIsGranted(() => global::Android.OS.Environment.IsExternalStorageManager);
        AndroidPermissions.SetHandler(() => activity.StartActivityForResult(new Intent(global::Android.Provider.Settings.ActionManageAllFilesAccessPermission), AndroidPermissions.RequestCode));
        isInitialized = true;
    }
#endif
}