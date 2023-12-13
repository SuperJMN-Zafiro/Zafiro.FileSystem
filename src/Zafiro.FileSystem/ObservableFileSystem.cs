﻿using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class ObservableFileSystem : IObservableFileSystem
{
    private readonly Subject<FileSystemChange> changed = new();
    private readonly IZafiroFileSystem fs;

    public ObservableFileSystem(IZafiroFileSystem fs)
    {
        this.fs = fs;
    }

    public IObservable<FileSystemChange> Changed => changed.AsObservable();
    public Task<Result<bool>> ExistsDirectory(ZafiroPath path) => fs.ExistDirectory(path);
    public Task<Result<bool>> ExistFile(ZafiroPath path) => fs.ExistFile(path);
    public Task<Result> DeleteFile(ZafiroPath path) => fs.DeleteFile(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.FileDeleted)));
    public Task<Result> DeleteDirectory(ZafiroPath path) => fs.DeleteDirectory(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.DirectoryDeleted)));

    public Task<Result> CreateFile(ZafiroPath path) => fs.CreateFile(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.FileCreated)));
    public IObservable<byte> GetFileContents(ZafiroPath path) => fs.GetFileContents(path);

    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes) => fs
        .SetFileContents(path, bytes)
        .TapIf(ExistFile(path).Map(b => !b), () => changed.OnNext(new FileSystemChange(path, Change.FileCreated)))
        .Tap(() =>
        {
            changed.OnNext(new FileSystemChange(path, Change.FileContentsChanged));
        });

    public Task<Result> CreateDirectory(ZafiroPath path) => fs.CreateDirectory(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.DirectoryCreated)));
    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path) => fs.GetFileProperties(path);
    public Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path) => fs.GetDirectoryProperties(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = default) => fs.GetFilePaths(path, ct);
    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default) => fs.GetDirectoryPaths(path, ct);
    public Task<Result<bool>> ExistDirectory(ZafiroPath path) => fs.ExistDirectory(path);
}

public enum Change
{
    FileCreated,
    FileDeleted,
    DirectoryCreated,
    FileContentsChanged,
    DirectoryDeleted
}

public record FileSystemChange(ZafiroPath Path, Change Change);