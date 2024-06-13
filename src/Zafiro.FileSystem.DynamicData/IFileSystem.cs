using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.DynamicData
{
    public interface IFileSystem
    {
        Task<Result<IRooted<DynamicDirectory>>> Get(ZafiroPath path);
    }
}