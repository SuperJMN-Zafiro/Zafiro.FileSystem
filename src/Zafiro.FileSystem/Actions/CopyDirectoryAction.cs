using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Actions;

public class CopyDirectoryAction : IAction<LongProgress>
{
    private readonly IZafiroDirectory destination;
    private readonly BehaviorSubject<LongProgress> progress = new(new LongProgress());
    private readonly IZafiroDirectory source;

    public CopyDirectoryAction(IZafiroDirectory source, IZafiroDirectory destination)
    {
        this.source = source;
        this.destination = destination;
    }

    public IObservable<LongProgress> Progress => progress.AsObservable();

    public async Task<Result> Execute(CancellationToken ct)
    {
        var files = await source.GetFilesInTree();
        return await files.Map(zafiroFiles => CopyFiles(zafiroFiles, ct));
    }

    private async Task<Result> CopyFiles(IEnumerable<IZafiroFile> sources, CancellationToken ct)
    {
        var results = await sources
            .ToObservable()
            .SelectMany(src => GetDestinationFile(src).Map(dest => (src, dest)))
            .Successes()
            .Select(copy =>
            {
                return Observable.FromAsync(() => CopyFileAction.Create(copy.src, copy.dest).Map(action => action.Execute(ct))).Successes();
            })
            .Merge(3)
            .ToList();

        return results.Combine();
    }

    private Task<Result<IZafiroFile>> GetDestinationFile(IZafiroFile src)
    {
        return destination.FileSystem.GetFile(destination.Path.Combine(src.Path.MakeRelativeTo(source.Path)));
    }
}