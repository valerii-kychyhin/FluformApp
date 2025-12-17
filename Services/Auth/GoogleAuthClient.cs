using Microsoft.Maui.Authentication;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;
using FluformApp.Services.Auth;

namespace FluformApp.Services.Auth;

public class GoogleAuthClient
{
    private readonly IGoogleAuthApi _googleApi;

    public GoogleAuthClient(IGoogleAuthApi googleApi)
    {
        _googleApi = googleApi;
    }

    public async Task<string?> LoginAsync()
    {
        try
        {
            var redirectResponse = await _googleApi.GetRedirectUrlAsync();
            var redirectUrl = redirectResponse.redirect_url;

            if (string.IsNullOrEmpty(redirectUrl))
                return null;

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                // Windows — открываем WebView
                await Application.Current.MainPage.Navigation.PushAsync(new FluformApp.Views.Auth.GoogleLoginPage(redirectUrl, _googleApi));
                return null; // токен получим в GoogleLoginPage
            }
            else
            {
                // Android / iOS — WebAuthenticator
                var authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    new Uri(redirectUrl),
                    new Uri("myapp://callback")
                );

                if (authResult.Properties.TryGetValue("code", out var code))
                {
                    var callbackResult = await _googleApi.GetCallbackAsync(code);
                    Preferences.Set("auth_token", callbackResult.access_token);
                    return callbackResult.access_token;
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Google Login Error", ex.Message, "OK");
        }

        return null;
    }
}
