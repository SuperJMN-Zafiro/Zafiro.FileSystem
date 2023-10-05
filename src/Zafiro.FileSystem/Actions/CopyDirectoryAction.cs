﻿using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Actions;

public class CopyDirectoryAction : IAction<LongProgress>
{
    private readonly CompositeAction compositeAction;

    private CopyDirectoryAction(IZafiroDirectory source, IZafiroDirectory destination, CompositeAction compositeAction)
    {
        Source = source;
        Destination = destination;
        this.compositeAction = compositeAction;
        Progress = compositeAction.Progress;
    }

    public IZafiroDirectory Source { get; }
    public IZafiroDirectory Destination { get; }

    public IObservable<LongProgress> Progress { get; }

    public Task<Result> Execute(CancellationToken ct)
    {
        return compositeAction.Execute(ct);
    }

    public static async Task<Result<CopyDirectoryAction>> Create(IZafiroDirectory source, IZafiroDirectory destination)
    {
        var files = await source.GetFilesInTree().ConfigureAwait(false);

        var action = await files.Map(async zafiroFiles =>
        {
            var childrenTasks = await GetChildrenTasks(zafiroFiles, source, destination).ToList();
            return new CompositeAction(childrenTasks);
        }).ConfigureAwait(false);

        return action.Map(compositeAction => new CopyDirectoryAction(source, destination, compositeAction));
    }

    private static IObservable<IAction<LongProgress>> GetChildrenTasks(IEnumerable<IZafiroFile> sources, IZafiroDirectory source, IZafiroDirectory destination)
    {
        var results = sources
            .ToObservable()
            .SelectMany(src => GetDestinationFile(src, source, destination).Map(dest => (src, dest)))
            .Successes()
            .SelectMany(copy => Observable.FromAsync(() => CopyFileAction.Create(copy.src, copy.dest)).Successes());

        return results;
    }


    private static Task<Result<IZafiroFile>> GetDestinationFile(IZafiroFile src, IZafiroDirectory source, IZafiroDirectory destination)
    {
        return destination.FileSystem.GetFile(destination.Path.Combine(src.Path.MakeRelativeTo(source.Path)));
    }
}