﻿using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Comparer;

namespace Zafiro.FileSystem.Synchronizer;

public class DeleteFilesOnlyOnRightSide : IFileAction
{
    private readonly CompositeAction composite;

    private DeleteFilesOnlyOnRightSide(IEnumerable<IFileAction> copyFileActions)
    {
        composite = new CompositeAction(copyFileActions.Cast<IAction<LongProgress>>().ToList());
        Progress = composite.Progress;
    }

    public IObservable<LongProgress> Progress { get; }
    public Task<Result> Execute(CancellationToken cancellationToken)
    {
        return composite.Execute(cancellationToken);
    }

    public static async Task<Result<DeleteFilesOnlyOnRightSide>> Create(IZafiroDirectory source, IZafiroDirectory destination)
    {
        var childActions = await new FileSystemComparer().Diff(source, destination)
            .Bind(diffs =>
            {
                var results = diffs.OfType<RightOnly>().Select(rightDiff => rightDiff.Get(destination).Bind(DeleteFileAction.Create));
                return results.Combine();
            });

        return childActions.Map<IEnumerable<DeleteFileAction>, DeleteFilesOnlyOnRightSide>(r => new DeleteFilesOnlyOnRightSide(r.Cast<IFileAction>().ToList()));
    }
}