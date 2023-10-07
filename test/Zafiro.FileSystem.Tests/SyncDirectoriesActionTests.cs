using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CSharpFunctionalExtensions;
using MoreLinq.Experimental;
using Serilog;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests;

public class SyncDirectoriesActionTests
{
    [Fact]
    public async Task Sync()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["C:\\Source\\File1.txt"] = new("Hi"),
            ["C:\\Source\\File2.txt"] = new("How"),
            ["C:\\Destination\\File1.txt"] = new("WILL BE REPLACED")
        });

        var local = new LocalFileSystem(mockFileSystem, Maybe<ILogger>.None);
        var r = await local.GetDirectory("C:/Source").CombineAndBind(local.GetDirectory("C:/Destination"), (directory, zafiroDirectory) => SyncDirectoriesAction2.Create(directory, zafiroDirectory, Maybe<ILogger>.None));
        var execution = await r.Bind(s => s.Execute(CancellationToken.None));
        execution.Should().BeSuccess();

        mockFileSystem.GetFile("C:\\Destination\\File1.txt").TextContents.Should().Be("Hi");
        mockFileSystem.GetFile("C:\\Destination\\File2.txt").TextContents.Should().Be("How");
    }
}

public static class MockFileSystemMixin
{
    public static void ShouldBe(this MockFileSystem fileSystem, IDictionary<string, string> dict)
    {
        var constraints = dict.Select(pair => fileSystem.GetFile(pair.Key).TextContents.Should().Be(pair.Value));
    }
}

public class SyncDirectoriesAction2 : IFileAction
{
    private readonly CompositeAction action;

    private SyncDirectoriesAction2(IZafiroDirectory source, IZafiroDirectory destination, CompositeAction compositeAction)
    {
        Source = source;
        Destination = destination;
        action = compositeAction;
        Progress = compositeAction.Progress;
    }

    public IZafiroDirectory Source { get; }
    public IZafiroDirectory Destination { get; }

    public IObservable<LongProgress> Progress { get; }

    public Task<Result> Execute(CancellationToken cancellationToken)
    {
        return action.Execute(cancellationToken);
    }

    public static async Task<Result<SyncDirectoriesAction2>> Create(IZafiroDirectory source, IZafiroDirectory destination, Maybe<ILogger> logger)
    {
        var childTasks = await source.GetFilesInTree()
            .ToObservable()
            .Successes()
            .SelectMany(files => files
                .Select(file => Observable.FromAsync(() => Sync(file, source, destination, logger)))
                .Merge(3)
                .Values())
            .ToList();

        var compositeAction = new CompositeAction(childTasks.Cast<IAction<LongProgress>>().ToList());
        return new SyncDirectoriesAction2(source, destination, compositeAction);
    }

    private static Task<Maybe<IFileAction>> Sync(IZafiroFile sourceFile, IZafiroDirectory sourceDirectory, IZafiroDirectory destinationDirectory, Maybe<ILogger> logger)
    {
        var relativeToSource = sourceFile.Path.MakeRelativeTo(sourceDirectory.Path);

        return destinationDirectory.FileSystem.GetFile(destinationDirectory.Path.Combine(relativeToSource))
            .Bind(destinationFile => CopyFileAction.Create(sourceFile, destinationFile))
            .Cast<CopyFileAction, IFileAction>()
            .TapError(e => logger.Execute(l => l.Information("Error synchronizing {File}. Reason: \"{Error}\"", sourceFile, e)))
            .AsMaybe();
    }
}