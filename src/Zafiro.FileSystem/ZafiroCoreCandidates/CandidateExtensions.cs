using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.ZafiroCoreCandidates;

public static class CandidateExtensions
{
    public static IObservable<Result<TResult>> Combine<T, TResult>(this Func<Task<Result<T>>> t, Func<Task<Result<T>>> u, Func<T, T, TResult> selector)
    {
        var r = from a in Observable.FromAsync(t)
            from b in Observable.FromAsync(u)
            select new { a, b };

        return r.Select(arg => from n in arg.a from q in arg.b select selector(n, q));
    }
}