using DynamicData;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.FileSystem.DynamicData;

public static class DynamicMixin
{
    public static IObservable<IChangeSet<IFile, string>> AllFiles(this IDynamicDirectory directory) => directory.Files.MergeChangeSets(directory.AllDirectories().MergeManyChangeSets(x => x.AllFiles()));
    public static IObservable<IChangeSet<IDynamicDirectory, string>> AllDirectories(this IDynamicDirectory directory) => directory.Directories.MergeChangeSets(directory.Directories.MergeManyChangeSets(x => x.AllDirectories()));
}