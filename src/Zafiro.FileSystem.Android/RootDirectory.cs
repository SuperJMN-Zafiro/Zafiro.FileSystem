#if ANDROID
using Android.OS.Storage;
#endif

using Serilog;

namespace Zafiro.FileSystem.Android;

public class RootDirectory : IZafiroDirectory
{
    private readonly System.IO.Abstractions.IFileSystem regularFileSystem;
    private readonly Maybe<ILogger> logger;

    public RootDirectory(IFileSystem fileSystem, System.IO.Abstractions.IFileSystem regularFileSystem, Maybe<ILogger> logger)
    {
        this.regularFileSystem = regularFileSystem;
        this.logger = logger;
        FileSystem = fileSystem;
    }

    public ZafiroPath Path => ZafiroPath.Empty;
    public IFileSystem FileSystem { get; }
    public async Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories()
    {
#if ANDROID
        return Result.Try(() =>
        {
            var storageVolumes = StorageManager.FromContext(Application.Context)!.StorageVolumes.ToList();
            return storageVolumes;
        }).Map(list =>
        {
            return list.Select(volume =>
            {
                var directoryInfo = regularFileSystem.DirectoryInfo.New(volume.Directory!.Path);
                return (IZafiroDirectory) new AndroidDirectory(directoryInfo, logger, FileSystem);
            });
        });
#else
        return Result.Failure<IEnumerable<IZafiroDirectory>>("Only supported in Android");
#endif
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles() => Task.FromResult(Result.Success(Enumerable.Empty<IZafiroFile>()));

    public Task<Result> Delete() => Task.FromResult(Result.Failure("Can't delete root"));
}