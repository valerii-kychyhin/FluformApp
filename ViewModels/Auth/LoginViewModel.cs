using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluformApp.Models.Auth;
using FluformApp.Services.Auth;
using FluformApp.Services.Profile;
using FluformApp.Views.Profile;
using FluformApp.ViewModels.Profile;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;

namespace FluformApp.ViewModels.Auth
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthClient _authClient;

        public LoginViewModel(AuthClient authClient)
        {
            _authClient = authClient;
        }

        [ObservableProperty] private string email;
        [ObservableProperty] private string password;
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private bool isPasswordHidden = true;
        [ObservableProperty] private bool rememberMe;

        // ===== Toggle password visibility =====
        [RelayCommand]
        private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;

        // ===== Email login =====
        [RelayCommand]
        private async Task Login()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var request = new LoginRequest
                {
                    Email = Email,
                    Password = Password
                };

                System.Diagnostics.Debug.WriteLine($"[Login] Sending request: {System.Text.Json.JsonSerializer.Serialize(request)}");

                var result = await _authClient.LoginAsync(request);

                if (result == null || string.IsNullOrWhiteSpace(result.token))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Couldn't log in", "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[Login] Received token: {result.token}");

                // ===== Save token in SecureStorage =====
                try
                {
                    await SecureStorage.SetAsync("auth_token", result.token);
                    await SecureStorage.SetAsync("token_type", result.token_type ?? "bearer");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Login] SecureStorage error: {ex}");
                }

                // ===== Create ProfileClient with token =====
                var profileApi = MauiProgram.Services.GetRequiredService<IProfileApi>();
                var profileClient = new ProfileClient(profileApi, result.token);

                // ===== Create ProfileViewModel =====
                var profileViewModel = new ProfileViewModel(profileClient, MauiProgram.Services);

                // ===== Navigate to ProfilePage =====
                if (Application.Current.MainPage is NavigationPage navPage)
                {
                    var profilePage = new ProfilePage(profileViewModel);
                    System.Diagnostics.Debug.WriteLine("[Login] Navigating to ProfilePage");
                    await navPage.PushAsync(profilePage);
                }
                else
                {
                    var profilePage = new ProfilePage(profileViewModel);
                    System.Diagnostics.Debug.WriteLine("[Login] Setting MainPage to NavigationPage(ProfilePage)");
                    Application.Current.MainPage = new NavigationPage(profilePage);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Login] Exception: {ex}");
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(CanLogin));
            }
        }

        // ===== Google login =====
        [RelayCommand]
        private async Task LoginWithGoogle()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var result = await _authClient.LoginWithGoogleAsync();

                if (result == null || string.IsNullOrWhiteSpace(result.token))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Couldn't authorize with Google", "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[GoogleLogin] Received token: {result.token}");

                // Save token
                try
                {
                    await SecureStorage.SetAsync("auth_token", result.token);
                    await SecureStorage.SetAsync("token_type", result.token_type ?? "bearer");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[GoogleLogin] SecureStorage error: {ex}");
                }

                // Navigate to ProfilePage
                var profileApi = MauiProgram.Services.GetRequiredService<IProfileApi>();
                var profileClient = new ProfileClient(profileApi, result.token);
                var profileViewModel = new ProfileViewModel(profileClient, MauiProgram.Services);
                var profilePage = new ProfilePage(profileViewModel);

                if (Application.Current.MainPage is NavigationPage navPage)
                {
                    System.Diagnostics.Debug.WriteLine("[GoogleLogin] Navigating to ProfilePage");
                    await navPage.PushAsync(profilePage);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[GoogleLogin] Setting MainPage to NavigationPage(ProfilePage)");
                    Application.Current.MainPage = new NavigationPage(profilePage);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Google login failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(CanLogin));
            }
        }

        public bool CanLogin => !string.IsNullOrWhiteSpace(Email) &&
                                !string.IsNullOrWhiteSpace(Password) &&
                                !IsBusy;

        partial void OnEmailChanged(string value) => OnPropertyChanged(nameof(CanLogin));
        partial void OnPasswordChanged(string value) => OnPropertyChanged(nameof(CanLogin));
    }
}