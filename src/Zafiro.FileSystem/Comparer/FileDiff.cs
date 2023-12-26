namespace Zafiro.FileSystem.Comparer;

public abstract record FileDiff
{
    public FileDiff()
    {
    }

    protected FileDiff(ZafiroPath Path)
    {
        this.Path = Path;
    }

    public ZafiroPath Path { get; init; }

    public void Deconstruct(out ZafiroPath Path)
    {
        Path = this.Path;
    }
}