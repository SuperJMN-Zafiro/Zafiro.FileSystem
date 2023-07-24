using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public static class FileExtensions
{
    public static async Task<Result<IEnumerable<IZafiroFile>>> GetFilesInTree(this IZafiroDirectory directory)
    {
        var myFiles = await Async.Await(directory.GetFiles);
        var filesFromSubdirs = await Async.Await(directory.GetDirectories)
            .Bind(async dirs =>
            {
                var p = await dirs.ToObservable()
                    .Select(x => Observable.FromAsync(x.GetFilesInTree))
                    .Merge(1)
                    .ToList();

                var results = p.Combine();
                return results.Map(x => x.SelectMany(u => u));
            });

        var filesInTree = 
            from p in myFiles 
            from q in filesFromSubdirs select p.Concat(q);
        return filesInTree;
    }
}