﻿#if ANDROID
using Android.Content;
using AppResult = Android.App.Result;

namespace Zafiro.FileSystem.Local.Android;

public static class AndroidPermissions
{
    private static Func<bool> GetIsGranted;
    private static TaskCompletionSource<Result>? tcs;
    public static readonly int RequestCode = 1;
    private static Action RequestEnable { get; set; }

    public static void SetIsGranted(Func<bool> isGranted)
    {
        GetIsGranted = isGranted;
    }

    public static void SetHandler(Action action)
    {
        RequestEnable = action;
    }

    public static Task<Result> Request()
    {
        var isGranted = GetIsGranted();

        if (tcs != null && !isGranted)
        {
            return Task.FromResult(Result.Failure("There's an pending request"));
        }

        tcs = new TaskCompletionSource<Result>();

        if (!isGranted)
        {
            RequestEnable();
        }
        else
        {
            tcs.SetResult(Result.Success());
        }

        return tcs.Task;
    }


    public static void OnActivityResult(int requestCode, AppResult resultCode, Intent? data)
    {
        if (tcs != null)
        {
            tcs.SetResult(GetIsGranted() ? Result.Success() : Result.Failure("Permission not granted"));
            tcs = null;
        }
    }

}
#endif