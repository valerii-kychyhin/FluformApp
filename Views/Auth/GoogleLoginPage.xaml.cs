using Microsoft.Maui.Controls;
using FluformApp.Services.Auth;
using Microsoft.Maui.Storage;
using System;

namespace FluformApp.Views.Auth;

public partial class GoogleLoginPage : ContentPage
{
    private readonly string _redirectUrl;
    private readonly IGoogleAuthApi _googleApi;

    public GoogleLoginPage(string redirectUrl, IGoogleAuthApi googleApi)
    {
        InitializeComponent();

        _redirectUrl = redirectUrl;
        _googleApi = googleApi;

        WebViewGoogle.Navigating += OnWebViewNavigating;
        WebViewGoogle.Source = _redirectUrl;
    }

    private async void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
    {
        try
        {
            if (e.Url.StartsWith("myapp://callback", StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;
                var uri = new Uri(e.Url);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var code = query["code"];
                await ProcessCodeAsync(code);
            }
            else if (e.Url.Contains("access_token"))
            {
                // Сервер вернул JSON прямо в окне
                e.Cancel = true;

                var response = await new System.Net.Http.HttpClient().GetStringAsync(e.Url);
                var callbackResult = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleCallbackResponse>(response);

                if (callbackResult != null && !string.IsNullOrEmpty(callbackResult.access_token))
                {
                    Preferences.Set("auth_token", callbackResult.access_token);

                    var profileApi = App.Services.GetRequiredService<FluformApp.Services.Profile.IProfileApi>();
                    var profileClient = new FluformApp.Services.Profile.ProfileClient(profileApi, callbackResult.access_token);
                    var profileVm = new FluformApp.ViewModels.Profile.ProfileViewModel(profileClient, App.Services);
                    Application.Current.MainPage = new NavigationPage(new FluformApp.Views.Profile.ProfilePage(profileVm));
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task ProcessCodeAsync(string? code)
    {
        if (string.IsNullOrEmpty(code))
            return;

        var googleApi = App.Services.GetRequiredService<IGoogleAuthApi>();
        var callbackResult = await googleApi.GetCallbackAsync(code);

        Preferences.Set("auth_token", callbackResult.access_token);

        var profileApi = App.Services.GetRequiredService<FluformApp.Services.Profile.IProfileApi>();
        var profileClient = new FluformApp.Services.Profile.ProfileClient(profileApi, callbackResult.access_token);
        var profileVm = new FluformApp.ViewModels.Profile.ProfileViewModel(profileClient, App.Services);
        Application.Current.MainPage = new NavigationPage(new FluformApp.Views.Profile.ProfilePage(profileVm));
    }
}
