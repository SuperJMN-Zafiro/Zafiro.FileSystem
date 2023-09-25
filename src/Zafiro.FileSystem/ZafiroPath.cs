using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public sealed class ZafiroPath : ValueObject
{
    public const char ChuckSeparator = '/';

    public ZafiroPath(string path) : this(path.Split(ChuckSeparator).ToArray())
    {
    }

    public ZafiroPath(IEnumerable<string> relativePathChunks)
    {
        RouteFragments = relativePathChunks;
    }

    public IEnumerable<string> RouteFragments { get; }

    public string Path => string.Join(ChuckSeparator, RouteFragments);

    public static implicit operator ZafiroPath(string[] chunks)
    {
        return new ZafiroPath(chunks);
    }

    public static implicit operator ZafiroPath(string path)
    {
        return new ZafiroPath(path);
    }

    public static implicit operator string(ZafiroPath path)
    {
        return path.ToString();
    }

    public override string ToString()
    {
        return string.Join(ChuckSeparator, RouteFragments);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Path;
    }
}