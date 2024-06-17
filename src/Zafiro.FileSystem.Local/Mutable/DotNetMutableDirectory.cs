using System.Reactive.Subjects;
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

    public string Name => Directory.Name;

    public Task<Result<IEnumerable<INode>>> Children()
    {
        return Directory.Children();
    }

    public Task<Result<IEnumerable<IMutableNode>>> MutableChildren()
    {
        var mutableChildren = Directory.Children().MapEach(x =>
        {
            if (x is DotNetDirectory d)
            {
                return (IMutableNode)new DotNetMutableDirectory(d);
            }

            if (x is DotNetFile f)
            {
                return new DotNetMutableFile(f);
            }

            throw new NotSupportedException();
        });
        
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

    public async Task<Result<IMutableFile>> CreateFile(string name)
    {
        var path = Directory.DirectoryInfo.FileSystem.Path.Combine(Directory.DirectoryInfo.FullName, name);
        return Result.Try(() => Directory.DirectoryInfo.FileSystem.FileInfo.New(path))
            .TapTry(fi =>
            {
                var fileSystemStream = fi.Create();
                fileSystemStream.Dispose();
            })
            .Map(fi => new DotNetFile(fi))
            .Map(file => (IMutableFile)new DotNetMutableFile(file));
    }

    public async Task<Result<IMutableDirectory>> CreateDirectory(string name)
    {
        return Result.Try(() => Directory.DirectoryInfo.CreateSubdirectory(name))
            .TapTry(fi => fi.Create())
            .Map(fi => new DotNetDirectory(fi))
            .Map(file => (IMutableDirectory)new DotNetMutableDirectory(file));
    }

    public bool IsHidden => (Directory.DirectoryInfo.Attributes & FileAttributes.Hidden) != 0;
}