using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Core.Mixins;

namespace Zafiro.FileSystem;

public class CopyAction : ISyncAction
{
    public CopyAction(IZafiroFile source, IZafiroFile destination)
    {
        Source = source;
        Destination = destination;
    }

    public IZafiroFile Source { get; set; }
    public IZafiroFile Destination { get; set; }
    public IObservable<RelativeProgress<int>> Progress { get; }
    public IObservable<Result> Sync()
    {
        return Observable
            .FromAsync(Source.GetContents)
            .WhereSuccess()
            .SelectMany(stream => Observable.Using(() => stream, x => Observable.FromAsync(() => Destination.SetContents(x))))
            .FirstAsync();
    }
}