using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class CompareFileSizeStrategy : IFileCompareStrategy
{
    public Task<Result<bool>> Compare(IZafiroFile one, IZafiroFile two)
    {
        var o = from l in one.Properties from r in two.Properties select new { l, r };
        return o.Map(x => x.l.Length == x.r.Length);
    }
}