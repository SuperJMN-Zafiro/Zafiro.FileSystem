using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public static class ZafiroPathMixin
{

    public static ZafiroPath Parent(this ZafiroPath path)
    { 
        return new ZafiroPath(path.RouteFragments.SkipLast(1));
    }

    public static string Name(this ZafiroPath path)
    { 
        return  path.RouteFragments.Last();
    }

    public static string NameWithoutExtension(this ZafiroPath path)
    {
        var last = path.RouteFragments.Last();
        var lastIndex = last.LastIndexOf('.');

        return lastIndex < 0 ? last : last[..lastIndex];
    }

    public static Maybe<string> Extension(this ZafiroPath path)
    {
        var name = path.Name();
        var dotPos = name.LastIndexOf('.');
        return dotPos < 0 ? Maybe<string>.None : name[(dotPos+1)..];
    }
}