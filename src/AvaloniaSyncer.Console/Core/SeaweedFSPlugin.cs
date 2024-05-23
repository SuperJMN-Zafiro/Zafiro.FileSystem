using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Console.Core;

public class SeaweedFSPlugin : IPlugin
{
    public string Name => "seaweedfs";
    public string FriendlyName  => "SeaweedFS";
    public Task<Result<ISyncFileSystem>> Create(string args) => SeaweedFS.Create(args).Map(x => (ISyncFileSystem)x);
}