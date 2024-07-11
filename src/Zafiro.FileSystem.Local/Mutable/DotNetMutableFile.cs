using System.Reactive.Concurrency;
using Zafiro.DataModel;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.Local.Mutable;

public class DotNetMutableFile : IMutableFile
{
    public DotNetMutableFile(IFileInfo fileInfo)
    {
        FileInfo = fileInfo;
    }

    public IFileInfo FileInfo { get; }

    public string Name => FileInfo.Name;

    public Task<Result> SetContents(IData data, CancellationToken cancellationToken, IScheduler? scheduler)
    {
        var result = Result.Try(() => FileInfo.Create());

        return result.Using(stream => data.DumpTo(stream, scheduler, cancellationToken));
    }

    public async Task<Result<IData>> GetContents()
    {
        return new FileInfoData(FileInfo);
    }

    public async Task<Result> Delete()
    {
        return Result.Try(() => FileInfo.Delete());
    }

    public bool IsHidden => (FileInfo.Attributes & FileAttributes.Hidden) != 0;

    public Task<Result<bool>> Exists()
    {
        throw new NotImplementedException();
    }

    public async Task<Result> Create()
    {
        return Result.Try(() =>
        {
            using (FileInfo.Create())
            {
            }
        });
    }
}