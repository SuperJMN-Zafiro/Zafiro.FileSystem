﻿using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.FileSystem2;

public class FileSystemRoot : IFileSystemRoot
{
    private readonly IObservableFileSystem obsFs;

    public FileSystemRoot(IObservableFileSystem obsFs)
    {
        this.obsFs = obsFs;
    }

    public IZafiroFile2 GetFile(ZafiroPath path) => new ZafiroFile2(path, this);
    public IZafiroDirectory2 GetDirectory(ZafiroPath path) => new ZafiroDirectory2(path, this);
    public Task<Result<IEnumerable<ZafiroFile2>>> GetFiles(ZafiroPath path) => obsFs.GetFilePaths(path).Map(paths => paths.Select(zafiroPath => new ZafiroFile2(zafiroPath, this)));
    public Task<Result<IEnumerable<ZafiroDirectory2>>> GetDirectories(ZafiroPath path) => obsFs.GetFilePaths(path).Map(paths => paths.Select(zafiroPath => new ZafiroDirectory2(zafiroPath, this)));
    public Task<Result<bool>> ExistDirectory(ZafiroPath path) => obsFs.ExistsDirectory(path);
    public Task<Result> CreateFile(ZafiroPath path) => obsFs.CreateFile(path);
    public IObservable<byte> Contents(ZafiroPath path) => obsFs.Contents(path);
    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes) => obsFs.SetFileContents(path, bytes);
    public Task<Result> CreateFolder(ZafiroPath path) => obsFs.CreateFolder(path);
    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path) => obsFs.GetFileProperties(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path) => obsFs.GetFilePaths(path);
    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path) => obsFs.GetDirectoryPaths(path);
    public IObservable<ZafiroPath> FileContentsChanged => obsFs.FileContentsChanged;
    public IObservable<ZafiroPath> FileCreated => obsFs.FileCreated;
    public IObservable<ZafiroPath> FolderCreated => obsFs.FolderCreated;
    public Task<Result<bool>> ExistsDirectory(ZafiroPath path) => obsFs.ExistsDirectory(path);
}