using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.FileSystem;

public static class ExceptionHandler
{
    public static string HandlePathAccessError(ZafiroPath path, Exception exception, Maybe<ILogger> logger)
    {
        logger.Execute(l => l.Error(exception, "Error while accessing {Path}", path));
        return $"Error while accessing {path}";
    }
}