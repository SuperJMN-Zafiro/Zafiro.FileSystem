namespace Zafiro.FileSystem.Android;

public static class AndroidPermissions
{
    private static Func<bool> IsGranted;
    private static TaskCompletionSource<Result> tcs;

    public static void SetIsGranted(Func<bool> isGranted)
    {
        IsGranted = isGranted;
    }
    
    public static void SetHandler(Action action)
    {
        Enable = action;
    }

    public static Action Enable { get; set; }
    public static readonly int RequestCode = 1;

    public static Task<Result> Request()
    {
        var isGranted = IsGranted();
        if (tcs != null && !isGranted)
        {
            return Task.FromResult(Result.Failure("There's an pending request"));
        }

        tcs = new TaskCompletionSource<Result>();

        if (!isGranted)
        {
            Enable();
        }
        else
        {
            tcs.SetResult(Result.Success());
        }

        return tcs.Task;
    }

    public static void OnResume()
    {
        if (tcs != null)
        {
            tcs.SetResult(IsGranted() ? Result.Success() : Result.Failure("Permission not granted"));
        }
    }
}