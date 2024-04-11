using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Comparer;

public static class Mixin
{
    public static Task<Result<IEnumerable<TResult>>> BindMany<TInput, TResult>(this Task<Result<IEnumerable<TInput>>> taskResult, Func<TInput, Result<TResult>> selector)
    {
        return taskResult.Bind(inputs => inputs.Select(selector).Combine());
    }

    public static Task<Result<IEnumerable<TResult>>> Map<TInput, TResult>(this Task<Result<IEnumerable<TInput>>> taskResult, Func<TInput, TResult> selector)
    {
        return taskResult.Map(inputs => inputs.Select(selector));
    }

    public static Task<Result<IEnumerable<TResult>>> BindMany<TInput, TResult>(this Task<Result<IEnumerable<TInput>>> taskResult, Func<TInput, Task<Result<TResult>>> selector)
    {
        return taskResult.Bind(inputs => inputs.Select(selector).Combine());
    }
}