using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography;
using CSharpFunctionalExtensions;
using Zafiro.IO;
using Zafiro.ProgressReporting;

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

    public Task<Result> Sync()
    {
        return Source.Copy(Destination, Maybe<IObserver<RelativeProgress<long>>>.From(progressSubject));
    }

    private IObservable<Result<ObservableStream>> GetInputStream(Stream stream)
    {
        return Observable.FromAsync(() => GetInputStream(Source, stream));
    }

    public async Task<Result> Copy(ObservableStream obs)
    {
        var onNext = obs.Positions.Select(l => new RelativeProgress<long>(obs.Length, l));
        using (onNext.Subscribe(progressSubject))
        {
            var contents = await Destination.SetContents(obs);
            progressSubject.OnNext(new RelativeProgress<long>(obs.Length, obs.Length));
            return contents;
        }
    }

    private static async Task<Result<ObservableStream>> GetInputStream(IZafiroFile zafiroFile, Stream stream)
    {
        if (!stream.CanSeek)
        {
            var size = await zafiroFile.Size();
            return size.Map(l => new ObservableStream(new AlwaysForwardStream(stream, l)));
        }

        return new ObservableStream(stream);
    }
}