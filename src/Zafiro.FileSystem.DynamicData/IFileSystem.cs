using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.DynamicData
{
    public interface IFileSystem
    {
        Task<Result<IRooted<IDynamicDirectory>>> Get(ZafiroPath path);
    }
}