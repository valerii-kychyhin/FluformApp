using Microsoft.Maui.Authentication;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;

namespace FluformApp.Services.Auth
{
    public class GoogleAuthClient
    {
        private readonly IGoogleAuthApi _googleApi;

        public GoogleAuthClient(IGoogleAuthApi googleApi)
        {
            _googleApi = googleApi;
        }

        /// <summary>
        /// Запускает Google OAuth и возвращает результат callback
        /// </summary>
        public async Task<GoogleCallbackResponse?> AuthenticateAsync()
        {
            try
            {
                var redirect = await _googleApi.GetRedirectUrlAsync();
                if (string.IsNullOrWhiteSpace(redirect.redirect_url))
                    return null;

                // Windows → WebView страница
                if (DeviceInfo.Platform == DevicePlatform.WinUI)
                {
                    var tcs = new TaskCompletionSource<GoogleCallbackResponse?>();
                    var page = new FluformApp.Views.Auth.GoogleLoginPage(redirect.redirect_url, _googleApi, tcs);
                    await Application.Current.MainPage.Navigation.PushAsync(page);
                    return await tcs.Task; // ждём результата от WebView
                }

                // Android / iOS
                var result = await WebAuthenticator.Default.AuthenticateAsync(
                    new Uri(redirect.redirect_url),
                    new Uri("myapp://callback")
                );

                if (!result.Properties.TryGetValue("code", out var code))
                    return null;

                // Получаем access_token с сервера
                var callback = await _googleApi.GetCallbackAsync(code);

                // Сохраняем токен в SecureStorage
                if (callback != null && !string.IsNullOrWhiteSpace(callback.access_token))
                {
                    await SecureStorage.SetAsync("access_token", callback.access_token);
                    await SecureStorage.SetAsync("token_type", callback.token_type ?? "bearer");
                }

                return callback;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Google Login Error",
                    ex.Message,
                    "OK");

                return null;
            }
        }
    }
}
