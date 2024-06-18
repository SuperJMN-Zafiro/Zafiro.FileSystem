namespace Zafiro.FileSystem.Local;

public record Rooted<T> : IRooted<T>
{
    public Rooted(ZafiroPath path, T directory)
    {
        Path = path;
        Value = directory;
    }

    public ZafiroPath Path { get; }
    public T Value { get; }
}