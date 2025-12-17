using FluformApp.ViewModels.Auth;
using FluformApp.Services.Auth;
using FluformApp.Services.Profile;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Firebase.CloudMessaging;
using FluformApp.ViewModels.Profile;
using FluformApp.Views.Profile;
using Microsoft.Maui.Controls;
using System;

namespace FluformApp.Views.Auth;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _vm;
    private readonly AuthClient _authClient;
    private readonly IGoogleAuthApi _googleApi;

    // Новый основной конструктор
    public LoginPage(LoginViewModel viewModel, AuthClient authClient, IGoogleAuthApi googleApi)
    {
        InitializeComponent();
        _vm = viewModel;
        _authClient = authClient;
        _googleApi = googleApi;
        BindingContext = _vm;
    }

    // Перегрузка для совместимости со старым кодом
    public LoginPage(LoginViewModel viewModel)
        : this(viewModel,
               App.Services.GetRequiredService<AuthClient>(),
               App.Services.GetRequiredService<IGoogleAuthApi>())
    {
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var profileApi = App.Services.GetRequiredService<IProfileApi>();
        var registerVm = new FluformApp.ViewModels.Auth.RegisterViewModel(_authClient, profileApi, _vm);
        await Navigation.PushAsync(new RegisterPage(registerVm));
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Forgot Password", "Password recovery is not implemented yet.", "OK");
    }

    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        // Используем AuthClient, чтобы сохранить токен и получить LoginResponse
        var loginResponse = await _authClient.LoginWithGoogleAsync();

        if (loginResponse != null && !string.IsNullOrWhiteSpace(loginResponse.token))
        {
            var profileApi = App.Services.GetRequiredService<IProfileApi>();
            var profileClient = new ProfileClient(profileApi, loginResponse.token);
            var profileVm = new ProfileViewModel(profileClient, App.Services);

            // Переход на страницу профиля
            Application.Current.MainPage = new NavigationPage(new ProfilePage(profileVm));
        }
        else
        {
            await DisplayAlert("Google Login", "Не удалось авторизоваться через Google.", "OK");
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
        Console.WriteLine($"FCM TOKEN: {token}");
    }
}