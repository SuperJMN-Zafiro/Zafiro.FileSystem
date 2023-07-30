using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface ISyncAction
{
    public IZafiroFile Source { get; set; }
    public IZafiroFile Destination { get; set; }
    public IObservable<RelativeProgress<int>> Progress { get; }
    public IObservable<Result> Sync();
}