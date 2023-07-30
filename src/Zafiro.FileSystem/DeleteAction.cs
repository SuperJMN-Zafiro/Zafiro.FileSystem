using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

internal class DeleteAction : ISyncAction
{
    public DeleteAction(IZafiroDirectory source, ZafiroPath diffPath)
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