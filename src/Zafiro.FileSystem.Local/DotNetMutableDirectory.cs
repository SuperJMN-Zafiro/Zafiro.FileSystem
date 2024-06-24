using System.Reactive.Subjects;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.Local;

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
                return (IMutableNode)new DotNetMutableDirectory(d);
            }

            if (x is DotNetFile f)
            {
                return new DotNetMutableFile(f.FileInfo);
            }

            throw new NotSupportedException();
        };
        var mutableChildren = FunctionalMixin.ManyMap(Directory.Children(), selector);

        return mutableChildren;
    }

    public Task<Result> AddOrUpdate(IFile data, ISubject<double>? progress = null)
    {
        var path = Directory.DirectoryInfo.FileSystem.Path.Combine(Directory.DirectoryInfo.FullName, data.Name);
        using (var stream = Directory.DirectoryInfo.FileSystem.File.Create(path))
        {
            return data.DumpTo(stream);
        }
    }

    public async Task<Result<IMutableFile>> Get(string name)
    {
        var path = Directory.DirectoryInfo.FileSystem.Path.Combine(Directory.DirectoryInfo.FullName, name);
        return Result.Try(() => Directory.DirectoryInfo.FileSystem.FileInfo.New(path))
            .Map(file => (IMutableFile)new DotNetMutableFile(file));
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

    public async Task<Result> Create()
    {
        return Result.Try(() => Directory.DirectoryInfo.Create());
    }

    public override string? ToString()
    {
        return Directory.ToString();
    }
}