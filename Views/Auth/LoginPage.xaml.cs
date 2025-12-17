using FluformApp.ViewModels.Auth;
using FluformApp.Services.Auth;
using FluformApp.Services.Profile;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Firebase.CloudMessaging;
using FluformApp.ViewModels.Profile;
using FluformApp.Views.Profile;

namespace FluformApp.Views.Auth;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _vm;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var authClient = App.Services.GetRequiredService<AuthClient>();
        var profileApi = App.Services.GetRequiredService<IProfileApi>();

        var registerVm = new FluformApp.ViewModels.Auth.RegisterViewModel(authClient, profileApi, _vm);

        await Navigation.PushAsync(new RegisterPage(registerVm));
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Forgot Password", "Password recovery is not implemented yet.", "OK");
    }

    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        var googleAuthClient = App.Services.GetRequiredService<GoogleAuthClient>();
        var token = await googleAuthClient.LoginAsync();

        if (!string.IsNullOrEmpty(token))
        {
            var profileApi = App.Services.GetRequiredService<IProfileApi>();
            var profileClient = new ProfileClient(profileApi, token);
            var profileVm = new ProfileViewModel(profileClient, App.Services);

            Application.Current.MainPage = new NavigationPage(new ProfilePage(profileVm));
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
        Console.WriteLine($"FCM TOKEN: {token}");
    }
}
