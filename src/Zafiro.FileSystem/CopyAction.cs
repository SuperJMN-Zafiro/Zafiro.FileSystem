using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Functional;
using Zafiro.IO;

namespace Zafiro.FileSystem;

public class CopyAction : ISyncAction
{
    private readonly ISubject<RelativeProgress<long>> progressSubject = new Subject<RelativeProgress<long>>();

    public CopyAction(IZafiroFile source, IZafiroFile destination)
    {
        Source = source;
        Destination = destination;
    }

    public IZafiroFile Source { get; set; }
    public IZafiroFile Destination { get; set; }
    public IObservable<RelativeProgress<long>> Progress => progressSubject.AsObservable();

    public IObservable<Result> Sync()
    {
        return Observable
            .FromAsync(Source.GetContents)
            .Successes()
            .SelectMany(stream => Observable.Using(() => new ObservableStream(stream), obs => Observable.FromAsync(async () =>
            {
                var onNext = obs.Positions.Select(l => new RelativeProgress<long>(obs.Length, l));
                using (onNext.Subscribe(progressSubject))
                {
                    var contents = await Destination.SetContents(obs);
                    progressSubject.OnNext(new RelativeProgress<long>(obs.Length, obs.Length));
                    return contents;
                }
            })))
            .FirstAsync();
    }
}