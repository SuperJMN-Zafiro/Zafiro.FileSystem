using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Console.Core;

public class DotnetFsPlugin : IPlugin
{
    public string Name => "local";
    public string FriendlyName => "Local Filesystem";
    public Task<Result<ISyncFileSystem>> Create(string args) => DotnetFs.Create().Map(x => (ISyncFileSystem)x);
}