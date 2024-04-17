using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Lightweight;

namespace Zafiro.FileSystem.VNext
{
    public interface IFileRepository
    {
        Task<Result<IEnumerable<IFile>>> GetFiles(ZafiroPath path);
        Task<Result<IEnumerable<IDirectory>>> GetDirectories(ZafiroPath path);
        Task<Result<IFile>> AddOrUpdate(ZafiroPath path, IFile data);
    }
}