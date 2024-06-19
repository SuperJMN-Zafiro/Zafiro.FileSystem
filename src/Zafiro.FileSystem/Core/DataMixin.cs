﻿using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Core;
using Zafiro.Mixins;
using Zafiro.Reactive;

namespace Zafiro.FileSystem.Core;

public static class DataMixin
{
    public static IObservable<Result> ChunkedDump(this IData data, Stream stream)
    {
        return data.Bytes.DumpTo(stream);
    }

    public static async Task<Result> DumpTo(this IData data, Stream stream)
    {
        var chuckResults = await data.ChunkedDump(stream).ToList();
        return chuckResults.Combine();
    }

    public static async Task<Result> DumpTo(this IData data, string path)
    {
        using (var stream = File.Open(path, FileMode.Create))
        {
            return await data.DumpTo(stream);
        }
    }

    public static byte[] Bytes(this IData data)
    {
        return data.Bytes.ToEnumerable().Flatten().ToArray();
    }
}