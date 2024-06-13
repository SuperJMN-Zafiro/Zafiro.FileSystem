﻿using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Actions;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableDirectory : IMutableNode, IAsyncDir
{
    Task<Result<IEnumerable<IMutableNode>>> MutableChildren();
    Task<Result> AddOrUpdate(IFile data, ISubject<double> progress);
    Task<Result<IMutableFile>> CreateFile(string name);
    Task<Result<IMutableDirectory>> CreateDirectory(string name); 
}