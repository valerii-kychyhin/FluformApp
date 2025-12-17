using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Maui.Authentication;

namespace FluformApp.Platforms.Android
{
    [Activity(
        NoHistory = true,
        Exported = true,
        LaunchMode = LaunchMode.SingleTop)]  // важно SingleTop для WebAuthenticator
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "myapp",
        DataHost = "callback")]
    public class CallbackActivity : WebAuthenticatorCallbackActivity
    {
        // ничего дописывать не нужно
    }
}