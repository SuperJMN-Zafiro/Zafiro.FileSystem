using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CSharpFunctionalExtensions;
using DynamicData;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using ClientDirectory = Zafiro.FileSystem.SeaweedFS.Filer.Client.Directory;

namespace Zafiro.FileSystem.SeaweedFS;

public class Directory : IMutableDirectory
{
    private Directory(ZafiroPath path, ISeaweedFS seaweedFS)
    {
        SeaweedFS = seaweedFS;
        Path = path;
    }

    public ZafiroPath Path { get; }
    public ISeaweedFS SeaweedFS { get; }
    public string Name => Path.Name();

    public static async Task<Result<Directory>> From(string path, ISeaweedFS seaweedFSClient)
    {
        return new Directory(path, seaweedFSClient);
    }

    private Task<Result<IEnumerable<IMutableNode>>> DirectoryToNodes(RootDirectory directory)
    {
        var entries = directory.Entries ?? new List<BaseEntry>();
        return AsyncResultExtensionsLeftOperand.Combine(entries.Select(entry =>
        {
            return entry switch
            {
                ClientDirectory dir => Task.FromResult(Result.Success<IMutableNode>(new Directory(dir.FullPath[1..], SeaweedFS))),
                FileMetadata file => File.From(file.FullPath[1..], SeaweedFS).Map(fsFile => (IMutableNode)fsFile),
                _ => throw new ArgumentOutOfRangeException(nameof(entry))
            };
        }));
    }

    public bool IsHidden => false;
   
    public IObservable<IChangeSet<IMutableNode, string>> Children
    {
        get
        {
            return Observable.FromAsync(async ct => await SeaweedFS.GetContents(Path, ct))
                .Successes()
                .SelectMany(x => DirectoryToNodes(x))
                .Successes()
                .ToObservableChangeSet(x => x.Name);
        }
    }

    public Task<Result<IMutableFile>> CreateFile(string entryName)
    {
        return Task.FromResult<Result<IMutableFile>>(new File(Path.Combine(entryName), SeaweedFS));
    }

    public Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        var directoryPath = Path.Combine(name);
        return SeaweedFS.CreateFolder(directoryPath).Map(() => (IMutableDirectory)new Directory(directoryPath, SeaweedFS));
    }

    public Task<Result> DeleteFile(string name)
    {
        return SeaweedFS.DeleteFile(Path.Combine(name));
    }

    public Task<Result> DeleteSubdirectory(string name)
    {
        return SeaweedFS.DeleteDirectory(Path.Combine(name));
    }
}