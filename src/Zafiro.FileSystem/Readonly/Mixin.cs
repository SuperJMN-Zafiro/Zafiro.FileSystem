using System.Reactive.Concurrency;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.Misc;

namespace Zafiro.FileSystem.Readonly;

public static class Mixin
{
    public static IEnumerable<IFile> Files(this IDirectory directory)
    {
        return directory.Children.OfType<IFile>();
    }

    public static IEnumerable<IRootedFile> RootedFiles(this IDirectory directory)
    {
        return directory.RootedFilesRelativeTo(ZafiroPath.Empty);
    }
    
    public static IEnumerable<IFile> AllFiles(this IDirectory directory)
    {
        return directory.Files().Concat(directory.Directories().SelectMany(d => d.AllFiles()));
    }

    public static IEnumerable<IRootedFile> RootedFilesRelativeTo(this IDirectory directory, ZafiroPath path)
    {
        var myFiles = directory.Children.OfType<IFile>().Select(file => new RootedFile(path, file));
        var filesInSubDirs = directory.Directories().SelectMany(d => d.RootedFilesRelativeTo(path.Combine(d.Name)));

        return myFiles.Concat(filesInSubDirs);
    }

    public static IEnumerable<IDirectory> Directories(this IDirectory directory)
    {
        return directory.Children.OfType<IDirectory>();
    }

    public static Task<Result> SafeCopy(this IFile file, IMutableDirectory output, Maybe<ILogger> logger, IScheduler? scheduler, CancellationToken cancellationToken)
    {
        var name = StringUtil.GetNewName(
            file.Name, 
            name => output.HasFile(name)
                .TapIf(exists => exists, () => logger.Execute(l => l.Warning("Filename '{Name}' exists. Choosing a new one.", name)))
                .Map(x => !x),
            (s, i) =>
            {
                var zafiroPath = (ZafiroPath)s;
                var newName = zafiroPath.NameWithoutExtension() + "_conflict_" + i + "." + zafiroPath.Extension();
                return newName;
            });
        
        return name
            .Tap(n => logger.Execute(l => l.Debug("Copying file {File} to {Name}", file, n)) )
            .Bind(finalName => output.CreateFileWithContents(finalName, file, scheduler: scheduler, cancellationToken));        
    }
}