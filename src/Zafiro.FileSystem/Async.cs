using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public static class Async
{
    public static async Task<Result<T>> Await<T>(Func<Task<Result<T>>> func)
    {
        return await func();
    }

    public static async Task<T> Await<T>(Func<Task<T>> func)
    {
        return await func();
    }
}