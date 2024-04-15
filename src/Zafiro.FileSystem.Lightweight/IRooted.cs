namespace Zafiro.FileSystem.Lightweight;

public interface IRooted<out T> where T : INamed
{
    ZafiroPath Path { get; }
    T Rooted { get; }
}