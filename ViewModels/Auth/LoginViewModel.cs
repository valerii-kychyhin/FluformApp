using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluformApp.Models.Auth;
using FluformApp.Services.Auth;
using Microsoft.Maui.Controls;
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

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isPasswordHidden = true;

        [ObservableProperty]
        private bool rememberMe;

        // ===== TOGGLE PASSWORD VISIBILITY =====
        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }

        // ===== EMAIL LOGIN =====
        [RelayCommand]
        private async Task Login()
        {
            if (!CanLogin) return;

            IsBusy = true;

            try
            {
                // минимальная валидация
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Email and Password cannot be empty", "OK");
                    return;
                }

                var result = await _authClient.LoginAsync(new LoginRequest
                {
                    Email = Email,
                    Password = Password
                });

                if (result == null || string.IsNullOrWhiteSpace(result.token))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Login failed. Check your credentials or network.", "OK");
                    return;
                }

                // Перейти на профиль после успешного входа
                await Shell.Current.GoToAsync("//profile");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(CanLogin));
            }
        }

        // ===== GOOGLE LOGIN =====
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

                await Shell.Current.GoToAsync("//profile");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Google login failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ===== CAN LOGIN PROPERTY =====
        public bool CanLogin =>
            !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(Password) &&
            !IsBusy;

        // ===== PROPERTY CHANGED =====
        partial void OnEmailChanged(string value)
        {
            OnPropertyChanged(nameof(CanLogin));
        }

        partial void OnPasswordChanged(string value)
        {
            OnPropertyChanged(nameof(CanLogin));
        }
    }
}