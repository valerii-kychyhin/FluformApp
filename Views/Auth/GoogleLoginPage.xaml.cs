using Microsoft.Maui.Controls;
using FluformApp.Services.Auth;
using FluformApp.Services.Profile;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FluformApp.Views.Auth;

public partial class GoogleLoginPage : ContentPage
{
    private readonly string _redirectUrl;
    private readonly IGoogleAuthApi _googleApi;
    private readonly TaskCompletionSource<GoogleCallbackResponse?> _tcs;

    public GoogleLoginPage(string redirectUrl, IGoogleAuthApi googleApi, TaskCompletionSource<GoogleCallbackResponse?> tcs)
    {
        InitializeComponent();
        _redirectUrl = redirectUrl;
        _googleApi = googleApi;
        _tcs = tcs;

        WebViewGoogle.Navigating += OnWebViewNavigating;
        WebViewGoogle.Source = _redirectUrl;
    }

    private async void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("myapp://callback", StringComparison.OrdinalIgnoreCase))
        {
            e.Cancel = true;
            var uri = new Uri(e.Url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var code = query["code"];
            await ProcessCodeAsync(code);
        }
    }

    private async Task ProcessCodeAsync(string? code)
    {
        if (string.IsNullOrEmpty(code))
        {
            _tcs.SetResult(null);
            return;
        }

        try
        {
            var callbackResult = await _googleApi.GetCallbackAsync(code);

            if (!string.IsNullOrEmpty(callbackResult.access_token))
            {
                // Сохраняем токен в SecureStorage
                await SecureStorage.SetAsync("access_token", callbackResult.access_token);
                await SecureStorage.SetAsync("token_type", callbackResult.token_type ?? "bearer");

                // Загружаем профиль и открываем страницу
                var profileApi = App.Services.GetRequiredService<IProfileApi>();
                var profileClient = new ProfileClient(profileApi, callbackResult.access_token);
                var profileVm = new FluformApp.ViewModels.Profile.ProfileViewModel(profileClient, App.Services);

                Application.Current.MainPage = new NavigationPage(new FluformApp.Views.Profile.ProfilePage(profileVm));
            }

            _tcs.SetResult(callbackResult);
        }
        catch (Exception)
        {
            _tcs.SetResult(null);
        }
        finally
        {
            // Закрываем страницу WebView
            try
            {
                if (Application.Current.MainPage.Navigation.NavigationStack.Count > 1)
                    await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch { }
        }
    }
}