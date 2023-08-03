using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Functional;
using Zafiro.IO;
using Zafiro.ProgressReporting;
using ObservableEx = Zafiro.Mixins.ObservableEx;

namespace Zafiro.FileSystem;

public class CopyAction : ISyncAction
{
    private readonly ISubject<RelativeProgress<long>> progressSubject = new Subject<RelativeProgress<long>>();

    public CopyAction(IZafiroFile source, IZafiroFile destination)
    {
        Source = source;
        Destination = destination;
    }

    public IZafiroFile Source { get; }

    public IZafiroFile Destination { get; }

    public IObservable<RelativeProgress<long>> Progress => progressSubject.AsObservable();

    public IObservable<Result> Sync()
    {
        return Observable
            .FromAsync(Source.GetContents)
            .Successes()
            .SelectMany(stream => ObservableEx.Using(() => GetInputStream(Source, stream), obs =>
                Observable.FromAsync(async () =>
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

    private static async Task<ObservableStream> GetInputStream(IZafiroFile zafiroFile, Stream stream)
    {
        Stream inner;
        if (!stream.CanSeek)
        {
            var size = await zafiroFile.Size();
            inner = new AlwaysForwardStream(stream, size);
        }
        else
        {
            inner = stream;
        }

        return new ObservableStream(inner);
    }
}