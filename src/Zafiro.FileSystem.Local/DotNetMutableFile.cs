using Zafiro.DataModel;

namespace Zafiro.FileSystem.Local;

public class DotNetMutableFile : IMutableFile
{
    public DotNetFile DotnetFile { get; }

    public DotNetMutableFile(DotNetFile dotnetFile)
    {
        DotnetFile = dotnetFile;
    }

    public string Name => DotnetFile.Name;

    public async Task<Result> SetContents(IData data, CancellationToken cancellationToken)
    {
        using (var stream = DotnetFile.FileInfo.Create())
        {
            var dumpTo = await data.DumpTo(stream);
            return dumpTo;
        }
    }

    public async Task<Result<IData>> GetContents()
    {
        return DotnetFile;
    }

    public async Task<Result> Delete()
    {
        return Result.Try(() => DotnetFile.FileInfo.Delete());
    }

    public bool IsHidden => (DotnetFile.FileInfo.Attributes & FileAttributes.Hidden) != 0;
}