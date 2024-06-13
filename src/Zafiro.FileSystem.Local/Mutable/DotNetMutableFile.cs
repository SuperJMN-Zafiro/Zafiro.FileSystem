using Zafiro.DataModel;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.Local.Mutable;

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
}