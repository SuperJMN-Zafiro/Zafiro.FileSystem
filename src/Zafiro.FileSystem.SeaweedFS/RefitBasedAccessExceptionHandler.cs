using System.Net;
using CSharpFunctionalExtensions;
using Refit;
using Serilog;

namespace Zafiro.FileSystem.SeaweedFS;

internal static class RefitBasedAccessExceptionHandler
{
    public static string HandlePathAccessError(ZafiroPath path, Exception exception, Maybe<ILogger> logger)
    {
        if (exception is ApiException { StatusCode: HttpStatusCode.NotFound })
        {
            logger.Execute(l => l.Error(exception, "Error while accessing {Path}", path));
            return $"Path not found: {path}";
        }

        return ExceptionHandler.HandlePathAccessError(path, exception, logger);
    }
}