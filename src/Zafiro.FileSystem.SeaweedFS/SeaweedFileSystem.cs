﻿using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.IO;
using Zafiro.Mixins;
using Directory = Zafiro.FileSystem.SeaweedFS.Filer.Client.Directory;
using File = Zafiro.FileSystem.SeaweedFS.Filer.Client.File;

namespace Zafiro.FileSystem.SeaweedFS;

public class SeaweedFileSystem : IZafiroFileSystem
{
    private readonly ISeaweedFS seaweedFSClient;
    private readonly Maybe<ILogger> logger;

    public SeaweedFileSystem(ISeaweedFS seaweedFSClient, Maybe<ILogger> logger)
    {
        this.seaweedFSClient = seaweedFSClient;
        this.logger = logger;
    }

    public Task<Result> CreateFile(ZafiroPath path) => throw new NotImplementedException();

    public IObservable<byte> GetFileContents(ZafiroPath path)
    {
        return ObservableFactory.UsingAsync(() => seaweedFSClient.GetFileContent(ToServicePath(path)), stream => stream.ToObservable());
    }

    private string ToServicePath(ZafiroPath path)
    {
        return "/" + path;
    }

    private string ToZafiroPath(ZafiroPath path)
    {
        return path.ToString()[1..];
    }

    public async Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes)
    {
        return await Observable.Using(() => bytes.ToStream(), stream => Observable.FromAsync(ct => Result.Try(() => seaweedFSClient.Upload(path, stream, ct))));
    }

    public Task<Result> CreateDirectory(ZafiroPath path) => throw new NotImplementedException();

    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path)
    {
        return Result
            .Try(() => seaweedFSClient.GetFileMetadata(path, CancellationToken.None))
            .Map(f => new FileProperties(false, f.Crtime, f.FileSize));
    }

    public async Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path)
    {
        return Result
            .Success(new DirectoryProperties(false, DateTime.MinValue));
    }

    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = default)
    {
        return Result.Try(() => seaweedFSClient.GetContents(ToServicePath(path), ct))
            .Map(directory => directory.Entries?.OfType<File>().Select(d => d.FullPath) ?? Enumerable.Empty<string>())
            .Map(x => x.Select(s => (ZafiroPath)s));
    }

    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default)
    {
        return Result.Try(() => seaweedFSClient.GetContents(ToServicePath(path), ct))
            .Map(directory => directory.Entries?.OfType<Directory>().Select(d => ToZafiroPath(d.FullPath)) ?? Enumerable.Empty<string>())
            .Map(x => x.Select(s => (ZafiroPath)s));
    }

    public Task<Result<bool>> ExistDirectory(ZafiroPath path) => throw new NotImplementedException();

    public Task<Result<bool>> ExistFile(ZafiroPath path) => throw new NotImplementedException();

    public Task<Result> DeleteFile(ZafiroPath path) => throw new NotImplementedException();

    public Task<Result> DeleteDirectory(ZafiroPath path) => throw new NotImplementedException();
}