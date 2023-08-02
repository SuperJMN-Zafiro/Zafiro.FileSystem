using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

internal class DeleteAction : ISyncAction
{
    private readonly IZafiroFile file;

    public DeleteAction(IZafiroFile file)
    {
        this.file = file;
    }

    public IZafiroFile Source { get; set; }
    public IZafiroFile Destination { get; set; }
    public IObservable<RelativeProgress<long>> Progress => Observable.Return(new RelativeProgress<long>(1, 1));
    public IObservable<Result> Sync()
    {
        return Observable.FromAsync(() => file.Delete());
    }
}