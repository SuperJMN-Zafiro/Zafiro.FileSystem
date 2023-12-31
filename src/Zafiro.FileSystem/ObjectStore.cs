﻿using System.IO.IsolatedStorage;
using System.Text.Json;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class ObjectStore<T> where T : class
{
    private readonly string name;

    public ObjectStore(string name)
    {
        this.name = name;
    }

    public Task<Result> Save(T dto)
    {
        return OpenWrite(name).Bind(stream => Save(dto, stream));
    }

    private static Task<Result> Save(T dto, Stream stream)
    {
        return Result.Try(async () =>
        {
            await using (stream.ConfigureAwait(false))
            {
                await JsonSerializer.SerializeAsync(stream, dto).ConfigureAwait(false);
            }
        });
    }

    private static Task<Result<T>> Load(Stream stream)
    {
        return Result.Try(async () =>
        {
            await using (stream.ConfigureAwait(false))
            {
                return await JsonSerializer.DeserializeAsync<T>(stream).ConfigureAwait(false);
            }
        }).EnsureNotNull("Could not load");
    }

    public Task<Result<T>> Load()
    {
        return OpenRead(name).Bind(Load);
    }

    private static Result<Stream> OpenRead(string path)
    {
        return Result.Try(GetStore)
            .Bind(store =>
            {
                return Result.Try(() =>
                {
                    var isolatedStorageFileStream = new IsolatedStorageFileStream(path, FileMode.Open, store);
                    return (Stream) isolatedStorageFileStream;
                });
            });
    }

    private static Result<Stream> OpenWrite(string path)
    {
        return Result.Try(GetStore)
            .Map(store => (Stream)new IsolatedStorageFileStream(path, FileMode.Create, store));
    }

    private static IsolatedStorageFile GetStore()
    {
        var isolatedStorageFile = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
        return isolatedStorageFile;
    }
}