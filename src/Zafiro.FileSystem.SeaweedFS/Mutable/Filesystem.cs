using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS.Mutable;

public class Filesystem : IMutableFileSystem
{
    public ISeaweedFS SeaweedFS { get; }

    public Filesystem(ISeaweedFS seaweedFS)
    {
        SeaweedFS = seaweedFS;
    }
    
    public Task<Result<IMutableDirectory>> GetDirectory(ZafiroPath path)
    {
        return SeaweedFSDirectory.From(path, SeaweedFS).Map(dir => (IMutableDirectory)new Directory(dir));
    }

    public Task<Result<IMutableFile>> GetFile(ZafiroPath path)
    {
        return SeaweedFSFile.From(path, SeaweedFS).Map(file => (IMutableFile)new File(file.Name, SeaweedFS));
    }

    public ZafiroPath InitialPath => ZafiroPath.Empty;
}