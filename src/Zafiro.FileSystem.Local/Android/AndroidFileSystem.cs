#pragma warning disable CS0414 // Field is assigned but its value is never used
using System.IO.Abstractions;

#if ANDROID
using Environment = Android.OS.Environment;
using Android.Provider;
using Android.Content;
using Android.OS.Storage;
using AppResult = Android.App.Result;
#endif

namespace Zafiro.FileSystem.Local.Android;

#if ANDROID
public class AndroidFileSystem : ZafiroFileSystemBase
{
    public AndroidFileSystem(IFileSystem fileSystem) : base(fileSystem)
    {
    }

    public static void OnActivityResult(int requestCode, AppResult resultCode, Intent? data)
    {
        AndroidPermissions.OnActivityResult(requestCode, resultCode, data);
    }

    public static void Register(Activity activity)
    {
        AndroidPermissions.SetIsGranted(() => Environment.IsExternalStorageManager);
        AndroidPermissions.SetHandler(() => activity.StartActivityForResult(new Intent(Settings.ActionManageAllFilesAccessPermission), AndroidPermissions.RequestCode));
    }

    public override Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default)
    {
        return AndroidPermissions.Request().Bind(async () =>
        {
            if (path == ZafiroPath.Empty)
            {
                return Result
                    .Try(() => StorageManager.FromContext(Application.Context)!.StorageVolumes.ToList())
                    .Map(list => list.Select(volume => FileSystemToZafiroPath(volume.Directory!.Path)));
            }

            return await base.GetDirectoryPaths(path, ct).ConfigureAwait(false);
        });
    }

    public override Task<Result> CreateDirectory(ZafiroPath path) => AndroidPermissions.Request().Bind(() => base.CreateDirectory(path));

    public override Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = default) => AndroidPermissions.Request().Bind(() => base.GetFilePaths(path, ct));

    public override Task<Result> CreateFile(ZafiroPath path) => AndroidPermissions.Request().Bind(() => base.CreateFile(path));

    public override Task<Result<FileProperties>> GetFileProperties(ZafiroPath path) => AndroidPermissions.Request().Bind(() => base.GetFileProperties(path));

    public override Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes, CancellationToken cancellationToken) => AndroidPermissions.Request().Bind(() => base.SetFileContents(path, bytes));

    public override string PathToFileSystem(ZafiroPath path) => "/" + path;

    public override ZafiroPath FileSystemToZafiroPath(string fileSystemPath) => fileSystemPath[1..];

    public override Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path)
    {
        return AndroidPermissions.Request().Bind(() => Result.Try(() =>
        {
            var info = FileSystem.FileInfo.New(PathToFileSystem(path));
            var isHidden = path.RouteFragments.Count() != 1 && info.Attributes.HasFlag(FileAttributes.Hidden);
            return new DirectoryProperties(isHidden, info.CreationTime);
        }));
    }
}

#endif