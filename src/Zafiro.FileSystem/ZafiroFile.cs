﻿using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class ZafiroFile : IZafiroFile
{
    public ZafiroFile(ZafiroPath path, IFileSystemRoot fileSystemRoot)
    {
        Path = path;
        FileSystem = fileSystemRoot;
    }

    public IFileSystemRoot FileSystem { get; }
    public IObservable<byte> Contents => FileSystem.GetFileContents(Path);
    public Task<Result<bool>> Exists => FileSystem.ExistFile(Path);
    public ZafiroPath Path { get; }
    public Task<Result<IDictionary<HashMethod, byte[]>>> Hashes => FileSystem.GetHashes(Path);
    public Task<Result> Delete() => FileSystem.DeleteFile(Path);
    public Task<Result> SetContents(IObservable<byte> contents, CancellationToken cancellationToken = default) => FileSystem.SetFileContents(Path, contents, cancellationToken);
    public Task<Result<Stream>> GetData() => FileSystem.GetFileData(Path);
    public Task<Result> SetData(Stream stream, CancellationToken cancellationToken = default) => FileSystem.SetFileData(Path, stream, cancellationToken);
    public Task<Result<FileProperties>> Properties => FileSystem.GetFileProperties(Path);
    public override string ToString() => Path;
}