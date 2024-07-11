using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.Local.Mutable;

public class DotNetMutableDirectory : IMutableDirectory
{
    public DotNetDirectory Directory { get; }

    public DotNetMutableDirectory(DotNetDirectory directory)
    {
        Directory = directory;
    }

    public string Name => Directory.Name.Replace("\\", "");

    public Task<Result<IEnumerable<INode>>> Children()
    {
        return Directory.Children();
    }

    public Task<Result<IEnumerable<IMutableNode>>> MutableChildren()
    {
        Func<INode, IMutableNode> selector = x =>
        {
            if (x is DotNetDirectory d)
            {
                return new DotNetMutableDirectory(d);
            }

            if (x is DotNetFile f)
            {
                return new DotNetMutableFile(f.FileInfo);
            }

            throw new NotSupportedException();
        };
        
        var mutableChildren = Directory.Children().ManyMap(selector);

        return mutableChildren;
    }

    public async Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        return Result.Try(() => Directory.DirectoryInfo.CreateSubdirectory(name))
            .Map(fi => new DotNetDirectory(fi))
            .Map(file => (IMutableDirectory)new DotNetMutableDirectory(file));
    }

    public async Task<Result> Delete()
    {
        return Result.Try(() => Directory.DirectoryInfo.Delete());
    }

    public bool IsHidden
    {
        get
        {
            if (Directory.DirectoryInfo.Parent == null)
            {
                return false;
            }
            
            return (Directory.DirectoryInfo.Attributes & FileAttributes.Hidden) != 0;
        }
    }

    public Task<Result<bool>> Exists()
    {
        throw new NotImplementedException();
    }

    public async Task<Result> Create()
    {
        return Result.Try(() => Directory.DirectoryInfo.Create());
    }

    public override string? ToString()
    {
        return Directory.ToString();
    }
}