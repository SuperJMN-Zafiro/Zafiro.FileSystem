namespace Zafiro.FileSystem.Core;

public record Rooted<T> : IRooted<T>
{
    public Rooted(ZafiroPath path, T value)
    {
        Path = path;
        Value = value;
    }

    public ZafiroPath Path { get; }
    public T Value { get; }
}