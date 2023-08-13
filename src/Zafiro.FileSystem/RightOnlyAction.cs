using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.ProgressReporting;

namespace Zafiro.FileSystem;

public class RightOnlyAction : ISyncAction
{
    private readonly IZafiroFile file;

    public RightOnlyAction(IZafiroFile file)
    {
        this.file = file;
    }

    public IZafiroFile Source => file;
    public IObservable<RelativeProgress<long>> Progress => Observable.Return(new RelativeProgress<long>(1, 1));
    public Task<Result> Sync(CancellationToken cancellationToken)
    {
        return file.Delete();
    }
}