using Zafiro.DataModel;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.Local;

public class DotNetMutableFile : IMutableFile
{
    public IFileInfo FileInfo { get; }

    public DotNetMutableFile(IFileInfo fileInfo)
    {
        FileInfo = fileInfo;
    }

    public string Name => FileInfo.Name;

    public async Task<Result> SetContents(IData data, CancellationToken cancellationToken)
    {
        using (var stream = FileInfo.Create())
        {
            var dumpTo = await data.DumpTo(stream);
            return dumpTo;
        }
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