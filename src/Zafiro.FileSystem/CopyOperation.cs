namespace Zafiro.FileSystem;

public class CopyOperation
{
    public CopyOperation(IZafiroFile source, IObservable<double> progress)
    {
        Source = source;
        Progress = progress;
    }

    public IZafiroFile Source { get; }
    public IObservable<double> Progress { get; }
}