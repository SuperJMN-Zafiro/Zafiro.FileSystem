using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Actions;

namespace Zafiro.FileSystem.Actions;

public class DeleteDirectoryAction : IAction<LongProgress>
{
    private readonly BehaviorSubject<LongProgress> progressSubject = new(new LongProgress(0, 1));

    public DeleteDirectoryAction(IZafiroDirectory directory)
    {
        Directory = directory;
    }

    public IZafiroDirectory Directory { get; }

    public IObservable<LongProgress> Progress { get; }

    public async Task<Result> Execute(CancellationToken ct)
    {
        var result = await Directory.Delete();
        result.Tap(() => progressSubject.OnNext(new LongProgress(1, 1)));
        return result;
    }
}