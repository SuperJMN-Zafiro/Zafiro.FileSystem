using System.Reactive.Linq;
using Zafiro.DataModel;

namespace Zafiro.FileSystem.Actions;

public class ProgressWatcher : IData, IDisposable
{
    private readonly IDisposable subscription;
    public IData Source { get; }
    public IObserver<long> ProgressObserver { get; }

    public ProgressWatcher(IData source, IObserver<long> progressObserver)
    {
        Source = source;
        ProgressObserver = progressObserver;
        
        long total = 0;
        var bytes = Source.Bytes
            .Do(bytes =>
            {
                total += bytes.Length;
                ProgressObserver.OnNext(total);
            });
        
        subscription = bytes.Subscribe(_ => { }, onCompleted: () => ProgressObserver.OnCompleted());
        Bytes = bytes;
    }

    private void Add(int number)
    {
        ProgressObserver.OnNext(number);
    }

    public IObservable<byte[]> Bytes { get; }
    public long Length => Source.Length;

    public void Dispose()
    {
        subscription.Dispose();
    }
}