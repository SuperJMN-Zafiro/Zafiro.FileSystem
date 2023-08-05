using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public static class ResultObservableExtensions
{
    public static IObservable<Result<K>> Bind<T, K>(this IObservable<Result<T>> observable, Func<T, K> function)
    {
        return observable.Select(t => ResultExtensions.Map(t, function));
    }

    public static IObservable<Result<K>> Bind<T, K>(this IObservable<Result<T>> observable, Func<T, Task<K>> function)
    {
        return observable.SelectMany(t => t.Map(function));
    }

    public static IObservable<Result<K>> Bind<T, K>(this IObservable<Result<T>> observable, Func<T, Task<Result<K>>> function)
    {
        return observable.SelectMany(t => t.Bind(function));
    }

    public static IObservable<Result<K>> Bind<T, K>(this IObservable<Result<T>> observable, Func<T, Result<K>> function)
    {
        return observable.Select(t => t.Bind(function));
    }

    public static IObservable<Result<K>> MySelectMany<T, K>(this IObservable<Result<T>> observable, Func<T, IObservable<Result<K>>> function)
    {
        return observable
            .SelectMany(result => result.IsSuccess
                ? function(result.Value)
                : Observable.Return(Result.Failure<K>(result.Error)))
            .Catch((Exception ex) => Observable.Return(Result.Failure<K>(ex.Message)));
    }

    public static IObservable<Result> MySelectMany<T>(this IObservable<Result<T>> observable, Func<T, IObservable<Result>> function)
    {
        return observable
            .SelectMany(result => result.IsSuccess
                ? function(result.Value)
                : Observable.Return(Result.Failure(result.Error)))
            .Catch((Exception ex) => Observable.Return(Result.Failure(ex.Message)));
    }

    //public static Result<K> Combine<T, K>(this Result<T> one, Result<T> another, Func<T, T, K> combineFunction)
    //{
    //    return one.Bind(x => another.Map(y => combineFunction(x, y)));
    //}

    public static Result<K> Combine<T, Q, K>(this Result<T> one, Result<Q> another, Func<T, Q, K> combineFunction)
    {
        return one.Bind(x => another.Map(y => combineFunction(x, y)));
    }
}