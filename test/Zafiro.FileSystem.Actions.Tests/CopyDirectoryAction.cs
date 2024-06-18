using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable.Mutable;
using Zafiro.FileSystem.Readonly;
using Zafiro.Mixins;

namespace Zafiro.FileSystem.Actions.Tests;

public class CopyDirectoryAction
{
    public static Task<Result<IAction<LongProgress>>> Create(IDirectory source, IMutableDirectory destination)
    {
        return CreateTasks(source, destination)
            .Map(actions => new CompositeAction(actions.Cast<IAction<LongProgress>>().ToList()))
            .Map(action => (IAction<LongProgress>)action);
    }

    public static async Task<Result<IEnumerable<IFileAction>>> CreateTasks(IDirectory source, IMutableDirectory destination)
    {
        return await destination.CreateDirectory(source.Name)
            .Bind(async directory =>
            {
                var copyAllFiles = await source
                    .Files()
                    .Select(file => directory.CreateFile(file.Name).Map(f => (IFileAction)new CopyFileAction(file, f)))
                    .Combine();

                var copyAllDirs = await source
                    .Directories()
                    .Select(d => directory.CreateDirectory(d.Name)
                        .Bind(f => CreateTasks(d, f)))
                    .Combine()
                    .Map(x => x.Flatten());

                return copyAllFiles.CombineAndMap(copyAllDirs, (a, b) => a.Concat<IFileAction>(b));
            });
    }
}