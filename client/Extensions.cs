using MudBlazor;
using Shared;

namespace client;

public static class Extensions
{
    public static void HandleApiResult(this ISnackbar snackbar, ApiResult result)
    {
        if (result.Succeeded)
        {
            snackbar.Add(result.Message, Severity.Success);
            return;
        }

        snackbar.Add(result.Message, Severity.Error);
    }
    public static void SomethingUnexpected(this ISnackbar snackbar)
    {
        snackbar.Add("Something unexpected occured", Severity.Error);
    }
}