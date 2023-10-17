﻿using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;

namespace Zafiro.FileSystem.Synchronizer;

public class CopyLeftFilesStrategy : IStrategy
{
    public Task<Result<IFileAction>> Create(IZafiroDirectory source, IZafiroDirectory destination)
    {
        return CopyLeftFilesToRightSideAction.Create(source, destination).Cast(r => (IFileAction)r);
    }
}