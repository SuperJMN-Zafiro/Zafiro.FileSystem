using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

internal class CopyAction : ISyncAction
{
    public CopyAction(IZafiroDirectory source, ZafiroPath path, IZafiroDirectory destination)
    {
    }

    public IZafiroFile Source { get; set; }
    public IZafiroFile Destination { get; set; }
    public IObservable<RelativeProgress<int>> Progress { get; }
    public IObservable<Result> Sync()
    {
        throw new NotImplementedException();
    }
}