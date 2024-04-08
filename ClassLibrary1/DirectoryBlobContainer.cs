using System.IO.Abstractions;
using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public class DirectoryBlobContainer(Maybe<string> name, IDirectoryInfo directory) : IBlobContainer
{
    public string Name => name.GetValueOrDefault(directory.Name);
    public Task<Result<IEnumerable<IBlob>>> Blobs() => Task.FromResult(Result.Try(() => directory.GetFiles().Select(info => (IBlob)new FileBlob(info))));

    public Task<Result<IEnumerable<IBlobContainer>>> Children() => Task.FromResult(Result.Try(() => directory.GetDirectories().Select(info => (IBlobContainer)new DirectoryBlobContainer(info.Name, info))));
}