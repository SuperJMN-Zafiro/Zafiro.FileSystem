using CSharpFunctionalExtensions;
using Zafiro.ProgressReporting;

namespace Zafiro.FileSystem;

public interface ISyncAction
{
    public IObservable<RelativeProgress<long>> Progress { get; }
    public IObservable<Result> Sync();
}