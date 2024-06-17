using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.DynamicData
{
    public interface IFileSystem
    {
        Task<Result<IRooted<IMutableDirectory>>> Get(ZafiroPath path);
    }
}