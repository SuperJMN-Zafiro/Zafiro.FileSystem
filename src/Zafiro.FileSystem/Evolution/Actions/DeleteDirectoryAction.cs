using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.Actions;
using Zafiro.FileSystem.Actions;

namespace Zafiro.FileSystem.Evolution.Actions;

[PublicAPI]
public class DeleteDirectoryAction : IFileAction
{
    private readonly BehaviorSubject<LongProgress> progressSubject = new(new LongProgress(0, 1));

    public DeleteDirectoryAction(IZafiroDirectory2 directory)
    {
        Directory = directory;
        Progress = progressSubject.AsObservable();
    }

    public IZafiroDirectory2 Directory { get; }

    public IObservable<LongProgress> Progress { get; }

    public async Task<Result> Execute(CancellationToken cancellationToken)
    {
        var result = await Directory.Delete();
        result.Tap(() => progressSubject.OnNext(new LongProgress(1, 1)));
        return result;
    }
}