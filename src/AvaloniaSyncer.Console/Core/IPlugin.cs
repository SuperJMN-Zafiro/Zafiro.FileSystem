using CSharpFunctionalExtensions;

namespace AvaloniaSyncer.Console.Core;

public interface IPlugin
{
    string Name { get; }
    string FriendlyName { get; }
    Task<Result<ISyncFileSystem>> Create(string args);
}