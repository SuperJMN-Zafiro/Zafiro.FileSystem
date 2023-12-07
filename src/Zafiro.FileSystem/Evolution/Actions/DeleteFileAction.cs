using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.FileSystem.Actions;

namespace Zafiro.FileSystem.Evolution.Actions;

public class DeleteFileAction : IFileAction
{
    private readonly IZafiroFile2 file;
    private readonly BehaviorSubject<LongProgress> progress = new(new LongProgress(0, 1));

    public DeleteFileAction(IZafiroFile2 file)
    {
        this.file = file;
        Source = file;
    }

    public IZafiroFile2 Source { get; }

    public async Task<Result> Execute(CancellationToken cancellationToken)
    {
        var execute = await file.Delete().ConfigureAwait(false);
        execute.Tap(() => progress.OnNext(new LongProgress(1, 1)));
        return execute;
    }

    public IObservable<LongProgress> Progress => progress.AsObservable();

    public static Result<DeleteFileAction> Create(IZafiroFile2 zafiroFile)
    {
        return new DeleteFileAction(zafiroFile);
    }
}