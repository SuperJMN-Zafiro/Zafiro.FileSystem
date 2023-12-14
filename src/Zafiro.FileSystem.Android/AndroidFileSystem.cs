using System.IO.Abstractions;
using Zafiro.FileSystem.Local;
#pragma warning disable CS0414 // Field is assigned but its value is never used
#if ANDROID
using Android.Content;
using Android.OS.Storage;
using AppResult = Android.App.Result;
#endif

namespace Zafiro.FileSystem.Android;

public class AndroidFileSystem : ZafiroFileSystemBase
{
    private static bool isInitialized;

    public AndroidFileSystem(IFileSystem fileSystem) : base(fileSystem)
    {
    }

    public override async Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default)
    {
#if ANDROID
        return Result
            .Try(() => StorageManager.FromContext(Application.Context)!.StorageVolumes.ToList())
            .Map(list => list.Select(volume => FileSystemToZafiroPath(volume.Directory!.Path)));
#else
        return Result.Failure<IEnumerable<ZafiroPath>>("Only supported in Android");
#endif
    }
    public override string PathToFileSystem(ZafiroPath path) => "/" + path;

    public override ZafiroPath FileSystemToZafiroPath(string fileSystemPath) => throw new NotImplementedException();

    public override Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path) => throw new NotImplementedException();

    public override async Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = default) => Result.Try(() => FileSystem.Directory.GetFiles(PathToFileSystem(path)).Select(s => (ZafiroPath) s));

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