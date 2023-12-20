﻿using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IZafiroFileSystem
{
    Task<Result> CreateFile(ZafiroPath path);
    IObservable<byte> GetFileContents(ZafiroPath path);
    Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes);
    Task<Result> CreateDirectory(ZafiroPath path);
    Task<Result<FileProperties>> GetFileProperties(ZafiroPath path);
    Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path);
    Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = default);
    Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default);
    Task<Result<bool>> ExistDirectory(ZafiroPath path);
    Task<Result<bool>> ExistFile(ZafiroPath path);
    Task<Result> DeleteFile(ZafiroPath path);
    Task<Result> DeleteDirectory(ZafiroPath path);
}