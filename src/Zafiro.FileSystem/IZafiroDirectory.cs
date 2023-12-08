﻿using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IZafiroDirectory
{
    ZafiroPath Path { get; }
    Task<Result<bool>> Exists { get; }
    IFileSystemRoot FileSystem { get; }
    Task<Result<DirectoryProperties>> Properties { get; }
    Task<Result> Create();
    Task<Result<IEnumerable<IZafiroFile>>> GetFiles();
    Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories();
    Task<Result> Delete();
}