namespace Zafiro.FileSystem;

public interface IRooted<out T>
{
    ZafiroPath Path { get; }
    T Value { get; }
}