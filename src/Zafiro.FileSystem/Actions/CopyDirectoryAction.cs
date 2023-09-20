using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Actions;

public class CopyDirectoryAction : IAction
{
    private readonly IZafiroDirectory destination;
    private readonly BehaviorSubject<IProportionProgress> progress = new(new ProportionProgress());
    private readonly IZafiroDirectory source;

    public CopyDirectoryAction(IZafiroDirectory source, IZafiroDirectory destination)
    {
        this.source = source;
        this.destination = destination;
    }

    public IObservable<IProportionProgress> Progress => progress.AsObservable();

    public async Task<Result> Execute(CancellationToken ct)
    {
        var files = await source.GetFilesInTree();
        return await files.Map(zafiroFiles => CopyFiles(zafiroFiles, ct));
    }

    private async Task<Result> CopyFiles(IEnumerable<IZafiroFile> sources, CancellationToken ct)
    {
        var compositeAction = await sources
            .ToObservable()
            .SelectMany(src => GetDestinationFile(src).Map(dest => (src, dest)))
            .Successes()
            .Select(file => (IAction) new CopyFileAction(file.src, file.dest))
            .ToList()
            .Select(list => new CompositeAction(list))
            .FirstAsync();

        using var _ = compositeAction.Progress.Subscribe(progress);
        var executionResult = await compositeAction.Execute(ct);
        return executionResult;
    }

    private Task<Result<IZafiroFile>> GetDestinationFile(IZafiroFile src)
    {
        return destination.FileSystem.GetFile(destination.Path.Combine(src.Path.MakeRelativeTo(source.Path)));
    }
}