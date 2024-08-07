﻿using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableDirectory : IMutableNode, IAsyncDir
{
    Task<Result<IEnumerable<IMutableNode>>> MutableChildren();
    Task<Result> AddOrUpdate(IFile data, ISubject<double>? progress = null);
    Task<Result<IMutableFile>> Get(string name);
    Task<Result<IMutableDirectory>> CreateSubdirectory(string name);
    Task<Result> Delete();
}