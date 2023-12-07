using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.FileSystem2;

public interface IObservableFileSystem : IFileSystem2
{
    IObservable<ZafiroPath> FileContentsChanged { get; }
    IObservable<ZafiroPath> FileCreated { get; }
    IObservable<ZafiroPath> FolderCreated { get; }
    Task<Result<bool>> ExistsDirectory(ZafiroPath path);
}