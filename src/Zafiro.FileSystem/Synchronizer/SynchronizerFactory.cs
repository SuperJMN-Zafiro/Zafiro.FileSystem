﻿using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;

namespace Zafiro.FileSystem.Synchronizer;

public static class SynchronizerFactory
{
    public static async Task<Result<IAction<LongProgress>>> Create(IZafiroDirectory source, IZafiroDirectory destination, params IStrategy[] strategies)
    {
        var subActions = await GetSubactions(source, destination, strategies);
        var result = subActions
            .Map(actions => new CompositeAction(actions.Cast<IAction<LongProgress>>().ToList()))
            .Cast(x => (IAction<LongProgress>)x);

        return result;
    }

    private static async Task<Result<IEnumerable<IFileAction>>> GetSubactions(IZafiroDirectory source, IZafiroDirectory destination, params IStrategy[] strategies)
    {
        var subActions = await Task.WhenAll(strategies.Select(x => x.Create(source, destination)));
        return subActions.Combine();
    }
}