using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS;

public class FileSystem : IMutableFileSystem
{
    public ISeaweedFS SeaweedFS { get; }

    public FileSystem(ISeaweedFS seaweedFS)
    {
        SeaweedFS = seaweedFS;
    }
    
    public Task<Result<IMutableDirectory>> GetDirectory(ZafiroPath path)
    {
        return Directory.From(path, SeaweedFS).Map(s => (IMutableDirectory)s);
    }

    public Task<Result<IMutableFile>> GetFile(ZafiroPath path)
    {
        return File.From(path, SeaweedFS).Map(file => (IMutableFile)new File(file.Name, SeaweedFS));
    }

    public ZafiroPath InitialPath => ZafiroPath.Empty;
}