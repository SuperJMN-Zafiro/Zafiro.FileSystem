namespace Zafiro.FileSystem.Lightweight;

public interface IRooted<out T>
{
    ZafiroPath Path { get; }
    T Rooted { get; }
}